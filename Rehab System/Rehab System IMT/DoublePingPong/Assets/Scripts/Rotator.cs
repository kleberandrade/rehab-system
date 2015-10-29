using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{

	EnemyController enemy;
	Rigidbody rigidBody;
	LayerMask pickUpMask;

	void Awake()
	{
		pickUpMask = LayerMask.GetMask ("PickUp");
		enemy = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<EnemyController> ();
	}

	void Start()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		rigidBody.position = Vector3.ProjectOnPlane(enemy.FindImpact (pickUpMask).point, Vector3.up);
	}
}
