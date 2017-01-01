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
		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteAxisValue( elementID, GameAxis.X, GameAxisValue.POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteAxisValue( elementID, GameAxis.Z, GameAxisValue.POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteAxisValue( elementID, GameAxis.X, GameAxisValue.VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteAxisValue( elementID, GameAxis.Z, GameAxisValue.VELOCITY ) );

		body.MovePosition( masterPosition );
		body.velocity = masterVelocity;

		// Send locally controlled object position over network
		GameManager.GetConnection().SetLocalAxisValue( elementID, GameAxis.X, GameAxisValue.POSITION, body.position.x );
		GameManager.GetConnection().SetLocalAxisValue( elementID, GameAxis.Z, GameAxisValue.POSITION, body.position.z );
		GameManager.GetConnection().SetLocalAxisValue( elementID, GameAxis.X, GameAxisValue.VELOCITY, body.velocity.x );
		GameManager.GetConnection().SetLocalAxisValue( elementID, GameAxis.Z, GameAxisValue.VELOCITY, body.velocity.z );
	}
}

