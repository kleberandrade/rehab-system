using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{

	protected Rigidbody rigidBody;
	protected LayerMask pickUpMask;


	void Start()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}
}
