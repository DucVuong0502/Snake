using Fusion;
using UnityEngine;
using UnityEngine.UI;      // Thêm để dùng UI Button
using TMPro;              // Thêm để dùng TMP_Text nếu cần
using System.Linq;

public enum GameState
{
    WaitingForPlayers,
    Countdown,
    Playing
}

public class GameManager : NetworkBehaviour
{
    [Header("Settings")]
    public int minPlayersToStart = 2;
    public NetworkPrefabRef itemPrefab;
    public float countdownDuration = 3f;
    public float itemSpawnInterval = 1f;

    [Header("UI")]
    public Button startButton;       // Gán nút Start trong Inspector
    public TMP_Text countdownText;   // (Nếu có hiển thị đếm ngược)

    [Networked]
    public GameState CurrentState { get; set; } = GameState.WaitingForPlayers;

    [Networked]
    public float CountdownTimer { get; set; }

    private float lastItemSpawnTime = 0f;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        UpdateUI();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        switch (CurrentState)
        {
            case GameState.WaitingForPlayers:
                break;

            case GameState.Countdown:
                CountdownTimer -= Runner.DeltaTime;

                if (CountdownTimer <= 0f)
                {
                    StartGame();
                }
                break;

            case GameState.Playing:
                if (Runner.SimulationTime - lastItemSpawnTime >= itemSpawnInterval)
                {
                    SpawnItemRandom();
                    lastItemSpawnTime = Runner.SimulationTime;
                }
                break;
        }

        UpdateUI();

        if (CurrentState == GameState.Countdown && countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(CountdownTimer).ToString();
        }
    }

    void UpdateUI()
    {
        if (startButton != null)
            startButton.gameObject.SetActive(Object.HasStateAuthority && CurrentState == GameState.WaitingForPlayers);

        if (countdownText != null)
            countdownText.gameObject.SetActive(CurrentState == GameState.Countdown);
    }

    void StartCountdown()
    {
        CurrentState = GameState.Countdown;
        CountdownTimer = countdownDuration;
        lastItemSpawnTime = Runner.SimulationTime; // Đặt lại thời gian spawn
        // RPC_UpdateUIState(CurrentState);
    }

    void StartGame()
    {
        // RPC_UpdateUIState(GameState.Playing);
        RPC_EnableMovement();
        Debug.Log($"Starting game. Found SnakeHead3D count: {FindObjectsOfType<SnakeHead3D>().Length}");
    }

    public void SpawnItemRandom()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-10f, 10f),
            1f,
            Random.Range(-10f, 10f)
        );
        Runner.Spawn(itemPrefab, randomPos, Quaternion.identity);
    }

    void OnStartButtonClicked()
    {
        if (HasStateAuthority && CurrentState == GameState.WaitingForPlayers)
        {
            RPC_HostStartGame();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_HostStartGame()
    {
        if (CurrentState == GameState.WaitingForPlayers && Runner.ActivePlayers.Count() >= minPlayersToStart)
        {
            StartCountdown();
            Debug.Log($"Starting countdown on client with StateAuthority. Players: {Runner.ActivePlayers.Count()}");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateUIState(GameState state)
    {
        CurrentState = state;
        UpdateUI();
        PointSpawner[] spawners = FindObjectsOfType<PointSpawner>();
        foreach (var spawner in spawners)
        {
            if (spawner != null && spawner.HasStateAuthority)
            {
                spawner.OnGameStateChanged();
                Debug.Log($"Notified PointSpawner to update state to {state}");
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EnableMovement()
    {
        foreach (var playerObj in FindObjectsOfType<SnakeHead3D>())
        {
            if (playerObj.Object != null && playerObj.Object.HasInputAuthority)
            {
                playerObj.canMove = true;
                Debug.Log($"Enabled movement for SnakeHead3D {playerObj.Object.Id} with InputAuthority");
            }
        }
    }
}