using UnityEngine;
using System.Collections;

public class HideShow : MonoBehaviour {

	enum Axis {vertical, horizontal};

	private Vector2 hide, show, target;
	private RectTransform transf;
	private float time;
	public float slideTime, distance;

	[SerializeField] Axis axis;

	// Use this for initialization
	void Start () 
	{
		transf = GetComponent<RectTransform> ();
		show = transf.anchoredPosition;
		if (axis == Axis.horizontal)
			hide = transf.anchoredPosition + Vector2.Scale (transf.sizeDelta, Vector2.right * distance);
		else
			hide = transf.anchoredPosition + Vector2.Scale (transf.sizeDelta, Vector2.up * distance);
		transf.anchoredPosition = target = hide;
		slideTime = 2.5f;
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
