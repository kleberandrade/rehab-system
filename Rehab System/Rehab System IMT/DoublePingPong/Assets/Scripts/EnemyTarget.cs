using UnityEngine;
using System.Collections;

public class EnemyTarget : Rotator
{

	private BallController enemy;

	void Awake()
	{
		pickUpMask = LayerMask.GetMask ("PickUpWall");
        enemy = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<BallController>();
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		//rigidBody.position = Vector3.ProjectOnPlane(enemy.FindImpact (pickUpMask).point, Vector3.up);
	}
}
