using UnityEngine;
using System.Collections;

public class HideShow : MonoBehaviour {

	private Vector3 hide, show, target;
	private RectTransform transf;
	private float time;
	public float slideTime;

	// Use this for initialization
	void Start () 
	{
		transf = GetComponent<RectTransform> ();
		show = transf.position;
		hide = new Vector3 (transf.position.x + transf.sizeDelta.x * transf.lossyScale.x * 0.98f, transf.position.y, transf.position.z);
		transf.position = target = hide;
		time = 0f;

	}

	void Update()
	{
		transf.position = Vector3.Lerp (transf.position, target, time * 2f);
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
