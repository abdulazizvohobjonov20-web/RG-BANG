using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pooling system for efficient particle effects, projectiles, and damage popups
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;
    public static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                var poolObj = new GameObject("[ObjectPool]");
                _instance = poolObj.AddComponent<ObjectPool>();
                DontDestroyOnLoad(poolObj);
            }
            return _instance;
        }
    }

    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    public void RegisterPrefab(string key, GameObject prefab, int initialSize = 5)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new List<GameObject>(initialSize);
            prefabDictionary[key] = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                poolDictionary[key].Add(obj);
            }
        }
    }

    public GameObject GetObject(string key, Vector3 position = default, Quaternion rotation = default)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Pool key '{key}' not found!");
            return null;
        }

        GameObject obj = null;
        List<GameObject> pool = poolDictionary[key];

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                obj = pool[i];
                break;
            }
        }

        if (obj == null)
        {
            obj = Instantiate(prefabDictionary[key]);
            obj.transform.SetParent(transform);
            pool.Add(obj);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
