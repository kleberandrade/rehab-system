using UnityEngine;
using System.Collections;

public class PlayerEyes : MonoBehaviour 
{
    public Material[] eyes;
    public float repeatRate = 0.3f;
    [Range(0.0f, 1.0f)]
    public float threshold = 0.7f;

    private int index;

	void Start()
	{
        if (eyes.Length >= 2)
		    InvokeRepeating("EyesMovement", 0.0f, repeatRate);
	}

    void OnDisable()
    {
        CancelInvoke();
    }

    void EyesMovement()
	{
        GetComponent<Renderer>().material = eyes[Random.Range(0.0f, 1.0f) > threshold ? 0 : 1];   
	}
}
