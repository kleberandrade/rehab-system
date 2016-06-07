using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour 
{
    public int startNumber = 3;
    public string wordToStart = "GO!";
    public float wait = 3.0f;
    public float time = 1.0f;

    private int currentNumber;
    private bool isCounting;
    private TextMesh numberText;

    void Start()
    {
        numberText = GetComponent<TextMesh>();
    }

    void FixedUpdate()
    {
        if (isCounting)
        {
            if (currentNumber > 0)
                numberText.text = currentNumber.ToString();
            else
                numberText.text = wordToStart;
        }
        else
        {
            numberText.text = string.Empty;
        }
        
    }

    void OnEnable()
    {
        isCounting = false;
        currentNumber = startNumber;
        InvokeRepeating("Decrement", wait, time);
    }

    void OnDisable()
    {
        isCounting = false;
        CancelInvoke("Decrement");
    }
	
	void Decrement () 
    {
        if (!isCounting)
        {
            isCounting = true;
        }
        else
        {
            currentNumber -= 1;
            if (currentNumber == 0)
                CancelInvoke("Decrement");
        }
	}

}
