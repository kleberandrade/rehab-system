using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour 
{
    [Range(0.0f, 10.0f)]
    public float timeToNextScreen = 3.0f;
    public string nextScreen = string.Empty;
    public AudioClip soundClip;

	void Start () 
    {
        StartCoroutine("NextScreen", timeToNextScreen);
        if (soundClip)
            SoundManager.Instance.Play(soundClip);
	}

	IEnumerator NextScreen (float time) 
    {
        yield return new WaitForSeconds(time);
        ScreenManager.Instance.Load(nextScreen, false);
	}
}
