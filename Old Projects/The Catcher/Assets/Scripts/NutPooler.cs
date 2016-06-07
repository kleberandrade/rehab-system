using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NutPooler : MonoBehaviour 
{
    private static NutPooler instance = null;

    public static NutPooler Instance
    {
        get 
        {
            return instance;
        }
    }

    public GameObject pooledObject;
    public int pooledAmount = 10;
    public bool willGrow = true;

    private List<GameObject> pooledObjects;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject go = (GameObject)Instantiate(pooledObject);
            go.SetActive(false);
            pooledObjects.Add(go);
        }
	}
	
	public GameObject NextObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject go = (GameObject)Instantiate(pooledObject);
            pooledObjects.Add(go);
            return go;
        }

        return null;
    }
}
