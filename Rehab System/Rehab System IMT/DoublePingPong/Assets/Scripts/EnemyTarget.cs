using UnityEngine;
using System.Collections;

public class EnemyTarget : Rotator
{

	private EnemyController enemy;
	public bool isActive;

	void Awake()
	{
		isActive = false;
		pickUpMask = LayerMask.GetMask ("PickUpWall");
		enemy = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<EnemyController> ();
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (isActive)
			rigidBody.position = Vector3.ProjectOnPlane (enemy.FindImpact (pickUpMask).point, Vector3.up) + 0.5f * Vector3.down;
		else
			rigidBody.position = 100f * Vector3.forward;
	}
}
