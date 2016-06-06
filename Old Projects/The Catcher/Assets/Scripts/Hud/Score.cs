using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HudPointText))]
public class Score : Singleton<Score> 
{
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
