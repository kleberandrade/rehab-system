using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ObjectPrefab;

    [SerializeField]
    private int m_PooledAmount = 3;

    [SerializeField]
    private bool m_WillGrow = true;

    private List<GameObject> m_PooledObjects;

    void Start()
    {
        m_PooledObjects = new List<GameObject>();
        for (int i = 0; i < m_PooledAmount; i++)
            CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        GameObject go = (GameObject)Instantiate(m_ObjectPrefab);
        go.SetActive(false);
        m_PooledObjects.Add(go);
        return go;
    }

    public GameObject NextObject()
    {
        foreach (GameObject go in m_PooledObjects)
            if (!go.activeInHierarchy)
                return go;

        if (m_WillGrow)
            return CreateNewObject();

        return null;
    }
}