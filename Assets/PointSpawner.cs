using Fusion;
using UnityEngine;
using System.Collections;

public class PointSpawner : NetworkBehaviour
{
    [Header("Point Settings")]
    public NetworkPrefabRef pointPrefab; // Prefab point
    public float spawnInterval = 1f;     // 1 giây
    public int pointsPerInterval = 5;    // 5 point mỗi vòng

    public float areaSizeX = 10f; // vùng ±10
    public float areaSizeZ = 10f; // vùng ±10
    public float spawnY = 1f;     // Y cố định

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            StartCoroutine(SpawnPoints());
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

                Runner.Spawn(pointPrefab, randomPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
