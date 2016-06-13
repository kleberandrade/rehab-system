using UnityEngine;
using System.Collections;

public class HideShow : MonoBehaviour {

	private Vector3 hide, show;
	private RectTransform transform;
	private float time;

	public float slideTime;

	// Use this for initialization
	void Start () 
	{
		transform = GetComponent<RectTransform> ();
		show = transform.position;
		hide = new Vector3 (transform.position.x + transform.sizeDelta.x * transform.lossyScale.x, transform.position.y, transform.position.z);
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = Vector3.Lerp (show, hide, time * 2f);
		time += Time.deltaTime;
	}
}
