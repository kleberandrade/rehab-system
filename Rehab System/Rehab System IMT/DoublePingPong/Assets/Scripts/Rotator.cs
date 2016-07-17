using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{

	protected Rigidbody rigidBody;
	protected LayerMask pickUpMask;
	public Vector3 anguloEuler;

	void Start()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		//rigidBody.MoveRotation(Quaternion.Euler (anguloEuler));
		rigidBody.rotation = Quaternion.Euler (anguloEuler);
		//transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);rigidBody.rotation.eulerAngles + 
		//Quaternion.Euler (new Vector3 (15, 30, 45) * Time.deltaTime));
	}
}
