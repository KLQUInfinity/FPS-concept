using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{
    #region Singlton

    public static ObjectPooling Instance;

    void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> Pools;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;

    void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab, pool.parent);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            PoolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject obToSpawn = PoolDictionary[tag].Dequeue();
        obToSpawn.SetActive(false);

        obToSpawn.SetActive(true);
        obToSpawn.transform.position = pos;
        obToSpawn.transform.rotation = rot;

        PoolDictionary[tag].Enqueue(obToSpawn);

        return obToSpawn;
    }
}

[System.Serializable]
public class Pool
{
    public string Tag;
    public GameObject Prefab;
    public int Size;
    public Transform parent;
}