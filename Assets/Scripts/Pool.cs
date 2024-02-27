using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    static Pool instance;

    static readonly Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    static readonly List<GameObject> poolList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < transform.childCount; i++) 
        { 
            GameObject g = transform.GetChild(i).gameObject;
            prefabs.Add(g.name, g);
            Debug.Log(g.name);
        }
    }

    public static GameObject GetPoolObject(string poolName, Transform position)
    {
        foreach (GameObject g in poolList)
        {
            if (g.activeInHierarchy)
                continue;

            if (g.name != poolName)
                continue;

            g.transform.SetPositionAndRotation(position.position, position.rotation);
            g.SetActive(true);

            return g;
        }

        GameObject obj = Instantiate(prefabs[poolName], instance.transform);

        obj.name = poolName;
        obj.transform.SetPositionAndRotation(position.position, position.rotation);
        obj.SetActive(true);

        poolList.Add(obj);

        return obj;
    }
}
