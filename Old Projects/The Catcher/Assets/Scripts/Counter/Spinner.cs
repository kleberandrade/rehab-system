using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour 
{
    public float angleRotate = 36.0f;
    public float timeRotate = 0.2f;
    public float scaleSize = 5.0f;

    private Vector3 startScale;
    private float timeStart;
    private Transform spinnerTransform;

    void Start()
    {
        spinnerTransform = gameObject.transform;
    }

    void OnEnable()
    {
        InvokeRepeating("SpinnerRotate", 0f, timeRotate);
    }

    void OnDisable()
    {
        StopRotate();
    }

    void SpinnerRotate()
    {
        spinnerTransform.Rotate(Vector3.forward * angleRotate * (-1));
    }

    public void StopRotate()
    {
        CancelInvoke("SpinnerRotate");
    }
}