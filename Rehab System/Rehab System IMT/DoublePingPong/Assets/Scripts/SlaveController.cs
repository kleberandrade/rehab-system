using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class SlaveController : Controller 
{
	void FixedUpdate()
	{
		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY ) );

		Vector3 followingError = masterPosition - body.position;

		if( followingError.magnitude > rangeLimits.magnitude / 2.0f ) body.MovePosition( masterPosition );

		body.velocity = masterVelocity;

		//GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
		//GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
		//GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
		//GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );
	}

	public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

		Physics.Raycast( body.position, body.velocity, out boundaryHit, 60f, layerMask );

		return boundaryHit.point;
	}

	public void OnEnable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}
	public void OnDisable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}
}

