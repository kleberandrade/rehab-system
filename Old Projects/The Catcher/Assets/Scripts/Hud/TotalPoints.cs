using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HudPointText))]
public class TotalPoints : Singleton<TotalPoints>
{
    #region Singleton
    private static TotalPoints instance = null;

    public static TotalPoints Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TotalPoints>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != instance)
                Destroy(this.gameObject);
        }
    }
    #endregion

    private HudPointText hudPoint;

    public int Point
    {
        get
        {
            return hudPoint.Point;
        }
    }

    void Start()
    {
        hudPoint = GetComponent<HudPointText>();
    }

    public void Add(int point)
    {
        hudPoint.AddPoint(point);
    }
}
