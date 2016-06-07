using UnityEngine;
using System.Collections;

public class NutDisappear : MonoBehaviour 
{
    public float time = 1.0f;
    private bool disappear;
    private Vector3 startScale;
    private float startTime;

    void Awake()
    {
        startScale = transform.localScale;
    }

	void OnEnable () 
    {
        transform.localScale = startScale;
        disappear = false;
	}

    void Update()
    {
        if (disappear)
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, (Time.time - startTime) / time);
            if (Vector3.Distance(transform.localScale, Vector3.zero) <= 0.0f)
                SendMessageUpwards("Destroy");
        }
    }
	
	void Disappear () 
    {
        disappear = true;
        startTime = Time.time;
	}
}
