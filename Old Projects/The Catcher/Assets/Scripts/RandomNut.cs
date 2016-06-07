using UnityEngine;
using System.Collections;

public class RandomNut : MonoBehaviour
{
    private bool gameStart = false;

    void OnEnable()
    {
        Chronometer.OnStartTime += OnStartTime;
        Chronometer.OnStopTime += OnStopTime;
        Nut.OnGrounded += OnNutSpawn;
        Nut.OnCollected += OnNutSpawn;
    }

    void OnDisable()
    {
        Chronometer.OnStartTime -= OnStartTime;
        Chronometer.OnStopTime -= OnStopTime;
        Nut.OnGrounded -= OnNutSpawn;
        Nut.OnCollected -= OnNutSpawn;
    }

    void OnStopTime()
    {
        gameStart = false;
    }

	void OnStartTime () 
    {
        gameStart = true;
        Invoke("NutSpawn", 1.0f);
	}

    void OnNutSpawn()
    {
        Invoke("NutSpawn", 1.0f);
	}

    void NutSpawn()
    {
        if (gameStart)
            transform.SendMessage("Spawn");
    }
}
