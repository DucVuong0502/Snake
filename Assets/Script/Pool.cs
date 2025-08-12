using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPool : MonoBehaviour
{
    public NetworkPrefabRef prefabRef;
    public int initialSize = 10;

    private Queue<NetworkObject> pool = new Queue<NetworkObject>();
    private NetworkRunner runner;

    public void Init(NetworkRunner runner)
    {
        this.runner = runner;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = runner.Spawn(prefabRef, Vector3.zero, Quaternion.identity);
            obj.gameObject.SetActive(false); // Ẩn đi nhưng không despawn
            pool.Enqueue(obj);
        }
    }

    public NetworkObject Get(Vector3 pos, Quaternion rot)
    {
        NetworkObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = runner.Spawn(prefabRef, pos, rot);
        }

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(NetworkObject obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
