using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class BatController : Controller 
{
	void FixedUpdate()
	{
		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY ) );

		body.MovePosition( masterPosition );
		body.velocity = masterVelocity;

		// Send locally controlled object position over network
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );
	}
}

