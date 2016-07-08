using UnityEngine;
using System.Collections;

public class HideShow : MonoBehaviour {

	private Vector2 hide, show, target;
	private RectTransform transf;
	private float time;
	public float slideTime;

	// Use this for initialization
	void Start () 
	{
		transf = GetComponent<RectTransform> ();
		show = transf.anchoredPosition;
		hide = new Vector2 (transf.anchoredPosition.x + transf.sizeDelta.x * 0.98f, transf.anchoredPosition.y);
		transf.anchoredPosition = target = hide;
		slideTime = 2;
		time = 0f;
	}

	void Update()
	{
		transf.anchoredPosition = Vector2.Lerp (transf.anchoredPosition, target, time * slideTime);
		time += Time.deltaTime;
	}
	
	// Update is called once per frame
	public void Show () 
	{
		target = show;
		time = 0;
	}

	public void Hide () 
	{
		target = hide;
		time = 0;
	}
}
