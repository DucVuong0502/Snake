using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [Header("Settings")]
    public GameObject PlayerPrefab;
    public CameraManager camManager;

    [Header("Spawn Area")]
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(20, 1, 20);

    [Header("Player Limit")]
    public int maxPlayers = 2;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"PlayerJoined: {player.PlayerId}");

        if (Runner.SessionInfo.PlayerCount > maxPlayers)
        {
            if (Runner.IsServer)
            {
                Debug.Log($" {player.PlayerId} cút");
                Runner.Disconnect(player);
            }
            return;
        }

        if (player == Runner.LocalPlayer)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                areaCenter.y,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Vector3 spawnPos = areaCenter + randomOffset;

            Debug.Log($"Spawning player at {spawnPos}");

            var playerObj = Runner.Spawn(PlayerPrefab, spawnPos, Quaternion.identity, player);

            var snakeHead = playerObj.GetComponent<SnakeHead3D>();
            snakeHead.camManager = camManager;

            camManager.SetTarget(playerObj.transform);
        }
    }

}
