using UnityEngine;
using System.Collections.Generic;

public class Pool
{
    private List<GameObject> pooledObjects;
    private GameObject prefab;

    public Pool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetObject(Vector3 pos, Quaternion rot)
    {
        foreach (var obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Object.Instantiate(prefab, pos, rot);
        pooledObjects.Add(newObj);
        return newObj;
    }
}
