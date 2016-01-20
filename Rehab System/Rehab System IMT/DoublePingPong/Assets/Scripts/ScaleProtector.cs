using UnityEngine;
using System.Collections;

public class ScaleProtector : MonoBehaviour {

	public Transform parent;
	public Vector3 initialScale;
	public bool x, y, z;
	private float fx, fy, fz;

	// Use this for initialization
	void Start () 
	{
		parent = transform.parent.GetComponentInParent<Transform>();
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		fx = x ?  initialScale.x/parent.transform.localScale.x : transform.localScale.x;
		fy = y ?  initialScale.y/parent.transform.localScale.y : transform.localScale.y;
		fz = z ?  initialScale.z/parent.transform.localScale.z : transform.localScale.z;

		transform.localScale = new Vector3(fx, fy, fz);
	}
}
