using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class ObjectPool : NetworkBehaviour
{
    [Header("Pool Config:")]
    public GameObject prefab;
    public GameObject containerObjects;
    public int poolSize;

    public List<GameObject> pool;

    void Awake()
    {
        pool = new List<GameObject>();

        InstantiateObjects();
    }

    //[ServerRpc]
    private void InstantiateObjects()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, containerObjects.transform);
            //ServerManager.Spawn(obj);

            obj.name = prefab.name + "_" + i;

            pool.Add(obj);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        Debug.LogError("Pool: New object added.");

        // Instantiante another object if needed
        GameObject newObj = Instantiate(prefab, containerObjects.transform);
        //ServerManager.Spawn(newObj);
        pool.Add(newObj);
        return newObj;
    }
}