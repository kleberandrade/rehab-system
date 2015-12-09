using UnityEngine;
using System.Collections;

public class EnemyTarget : Rotator
{

	private EnemyController enemy;

	void Awake()
	{
		pickUpMask = LayerMask.GetMask ("PickUpWall");
		enemy = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<EnemyController> ();
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		rigidBody.position = Vector3.ProjectOnPlane(enemy.FindImpact (pickUpMask).point, Vector3.up);
	}
}
