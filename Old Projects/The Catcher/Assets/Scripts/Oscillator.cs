using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour 
{
    public OscillatorType oscillatorType = OscillatorType.Position;
    public float period = 1.0f;
    public float amplitude = 0.01f;
    public Vector3 direction = Vector3.one;
    public bool initRandomPeriod = false;
    
    private float startRandomPeriod = 0.0f;
    private Transform myTransform;
    private Vector3 origin;
    
    void Start()
    {
        myTransform = transform;

        if (initRandomPeriod)
            startRandomPeriod = Random.Range(0.0f, Mathf.PI);
       
        switch (oscillatorType)
        {
            case OscillatorType.Position:
                origin = myTransform.position;
                break;

            case OscillatorType.Scale:
                origin = myTransform.localScale;
                break;

            case OscillatorType.Rotation:
                origin = myTransform.eulerAngles;
                break;
        }
    }

    void Update()
    {
        switch(oscillatorType)
        {
            case OscillatorType.Position:
                myTransform.position = origin + direction * Mathf.Sin((startRandomPeriod + Time.time) * period) * amplitude;
                break;

            case OscillatorType.Scale:
                myTransform.localScale = origin + direction * Mathf.Sin((startRandomPeriod + Time.time) * period) * amplitude;
                break;

            case OscillatorType.Rotation:
                myTransform.eulerAngles = origin + direction * Mathf.Sin((startRandomPeriod + Time.time) * period) * amplitude;
                break;
        }
    }
}


public enum OscillatorType
{
    Position,
    Rotation,
    Scale
}