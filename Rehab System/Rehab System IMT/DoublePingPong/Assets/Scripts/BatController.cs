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
    /*void FixedUpdate()
    {
        // Get remotely controlled object position (z) and set it locally (x)
		float remoteInput = gameConnection.GetRemoteValue( (byte) elementID, NetworkAxis.X, NetworkValue.POSITION );

        float remotePosition = remoteInput * rangeLimits.x;

        //File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + remotePosition.ToString() + Environment.NewLine );

		//Debug.Log( "Bat " + elementID.ToString() + " moving to " + ( transform.right * Mathf.Clamp( remotePosition, -rangeLimits.x, rangeLimits.x ) ).ToString() );

		body.MovePosition( transform.right * Mathf.Clamp( remotePosition, -rangeLimits.x, rangeLimits.x ) + initialPosition );
    }*/

	void FixedUpdate()
	{
		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY ) );

		//Debug.Log( string.Format( "Bat {0} target: position {1} - velocity {2}", elementID, masterPosition, masterVelocity ) );

		body.MovePosition( masterPosition );
		body.velocity = masterVelocity;

		// Send locally controlled object position over network
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
		GameManager.GetConnection().SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );

		//File.AppendAllText( logFileName, Time.realtimeSinceStartup.ToString() + "\t" 
		//	+ ball.position.x.ToString() + "\t" + ball.position.z.ToString() + System.Environment.NewLine );
	}
}

