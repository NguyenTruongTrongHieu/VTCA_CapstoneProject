using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<string, ObjectPoolGeneric> poolDictionary = new Dictionary<string, ObjectPoolGeneric>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CreatePool(string key, int poolSize, GameObject prefab, Transform parent)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            var objectPool = new ObjectPoolGeneric(prefab, poolSize, parent);
            poolDictionary.Add(key, objectPool);
        }
        else
        {
            Debug.LogWarning($"Pool with key {key} already exists.");

        }
    }

    public GameObject GetObject(string key, Vector3 pos, Quaternion rotation, Transform parent, GameObject prefab)
    {
        if (poolDictionary.ContainsKey(key))
        {
            ObjectPoolGeneric objectPool = poolDictionary[key];
            return objectPool.GetObject(pos, rotation, parent);
        }
        else
        {
            // Optionally, you can create a new pool if it doesn't exist
            CreatePool(key, 1, prefab, parent);

            ObjectPoolGeneric objectPool = poolDictionary[key];
            return objectPool.GetObject(pos, rotation, parent);
        }
    }

    public IEnumerator GetObjectByTime(string key, Vector3 pos, Quaternion rotation, Transform parent, GameObject prefab, float time)
    {
        yield return new WaitForSeconds(time);
        GetObject(key, pos, rotation, parent, prefab);
    }

    public void ReturnObject(string key, GameObject obj) 
    {
        if (poolDictionary.ContainsKey(key))
        {
            ObjectPoolGeneric objectPool = poolDictionary[key];
            objectPool.ReturnObject(obj);
            obj.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning($"No pool found with key {key}");
            Destroy(obj); // Destroy the object if no pool exists
        }
    }

    public IEnumerator ReturnObjectByTime(string key, GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnObject(key, obj);
    }
}
