using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolGeneric
{
    private Queue<GameObject> objectPools = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPoolGeneric(GameObject prefab, int poolSize)
    {
        this.prefab = prefab;
        this.parent = null;

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    public ObjectPoolGeneric(GameObject prefab, int poolSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    GameObject CreateNewObject()
    {
        GameObject obj;
        if (parent == null)//Spawn object in the world
        {
            obj = Object.Instantiate(prefab);
        }
        else
        {
            obj = Object.Instantiate(prefab, parent);
        }

        //Add the object to the pool
        objectPools.Enqueue(obj);
        //Set the object inactive
        obj.gameObject.SetActive(false);
        return obj;
    }

    public GameObject GetObject(Vector3 pos, Quaternion rotation, Transform parent)
    {
        GameObject obj;
        if (objectPools.Count <= 0)
        {
            obj = CreateNewObject();
        }

        obj = objectPools.Dequeue();

        if (parent != null)
        {
            obj.transform.SetParent(parent);
        }

        obj.transform.position = pos;
        obj.transform.rotation = rotation;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        objectPools.Enqueue(obj);
    }
}
