using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

	public List<RawImage> logos;
	private float time;

	const float fadeIn = 0.5f;
	const float fadeOut = 0.2f;
	const float showTime = 3.0f;

	private int iLogo;

	void Start()
	{
		time = 0f;
		iLogo = 0;

		foreach (RawImage element in logos)
			element.CrossFadeAlpha (0f, 0f, false);
	}

	void Update () 
	{
		if (time == 0)
			logos[iLogo].CrossFadeAlpha (1f, fadeIn, false);

		time += Time.deltaTime;

		if ((time > showTime) && logos[iLogo].color.a == 1f)
			logos[iLogo].CrossFadeAlpha (0f, fadeOut, false);

		if (time > showTime + fadeOut)
		{
			iLogo++;
			if (iLogo == logos.Count)
				SceneManager.LoadScene ("MainGame");
			else
				time = 0f;
		}
	}
}
