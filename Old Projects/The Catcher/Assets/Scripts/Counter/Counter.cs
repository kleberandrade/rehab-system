using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour 
{
    public delegate void HandlerAction();
    public static event HandlerAction BeginCounter;
    public static event HandlerAction EndCounter;

    public float time = 7.0f;
    public AudioClip coutingClip;

    void OnEnable()
    {
        if (BeginCounter != null)
            BeginCounter();
    }

    void EndCounterEvent()
    {
        if (EndCounter != null)
            EndCounter();
    }

	void Disable () 
    {
        gameObject.SetActive(false);
	}

    void PlayCountingClip()
    {
        if (coutingClip)
            SoundManager.Instance.Play(coutingClip, 1.0f, false);
    }
}
