using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointSpawner : NetworkBehaviour
{
    public NetworkPrefabRef pointPrefab;
    public float spawnInterval = 1f;
    public int pointsPerInterval = 5;
    public float areaSizeX = 10f;
    public float areaSizeZ = 10f;
    public float spawnY = 1f;

    private NetworkPool pool;

    private Coroutine spawnRoutine;
    private List<NetworkObject> spawnedPoints = new List<NetworkObject>();
    private GameManager gameManager;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
                if (gameManager == null)
                {
                    Debug.LogError("GameManager not found in scene!");
                }
            }
            pool = gameObject.AddComponent<NetworkPool>();
            pool.prefabRef = pointPrefab;
            pool.initialSize = pointsPerInterval * 2;
            pool.Init(Runner);

            StartCoroutine(SpawnPoints());
            UpdateSpawningState();
        }
    }

    IEnumerator SpawnPoints()
    {
        while (true)
        {
            for (int i = 0; i < pointsPerInterval; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(-areaSizeX, areaSizeX),
                    spawnY,
                    Random.Range(-areaSizeZ, areaSizeZ)
                );

                var obj = pool.Get(randomPos, Quaternion.identity);

                // Truyền thông tin pool cho Point để nó tự trả về
                var point = obj.GetComponent<PointFromPool>();
                if (point != null)
                {
                    point.pool = pool;
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    private void UpdateSpawningState()
    {
        if (gameManager != null && gameManager.CurrentState == GameState.Playing)
        {
            if (spawnRoutine == null)
            {
                spawnRoutine = StartCoroutine(SpawnPoints());
                Debug.Log("Started spawning points.");
            }
        }
        else
        {
            StopSpawning();
            Debug.Log("Stopped spawning points due to game state.");
        }
    }
    public void OnGameStateChanged()
    {
        if (HasInputAuthority)
        {
            UpdateSpawingState();
        }
    }

    private void UpdateSpawingState()
    {

    }
    public void StartSpawning()
    {
        if (HasStateAuthority && gameManager != null && gameManager.CurrentState == GameState.Playing)
        {
            if (spawnRoutine == null)
            {
                spawnRoutine = StartCoroutine(SpawnPoints());
                Debug.Log("Started spawning points manually.");
            }
        }
    }
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            Debug.Log("Stopped spawning points.");
        }
    }
    public void ClearPoints()
    {
        foreach (var point in spawnedPoints)
        {
            if (point != null && point.HasStateAuthority)
            {
                Runner.Despawn(point);
            }
        }
        spawnedPoints.Clear();
        Debug.Log("Cleared all spawned points.");
    }


}
