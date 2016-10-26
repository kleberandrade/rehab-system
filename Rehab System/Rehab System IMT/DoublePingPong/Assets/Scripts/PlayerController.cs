using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class PlayerController : Controller 
{
	public const float ERROR_THRESHOLD = 0.35f;

	const float MOVE_INTERVAL = 1.0f;

	private InputAxis controlAxis = null;
	private bool helperEnabled = false;

	void FixedUpdate()
	{
		if( controlAxis != null )
		{
			float input = controlAxis.NormalizedPosition;
			//Debug.Log( "Input position: " + ( transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x ) ).ToString () );
			body.MovePosition( transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x ) + initialPosition );

			//File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + playerBody.position.z.ToString() + Environment.NewLine );

			// Send locally controlled object position over network
			gameConnection.SetLocalValue( elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
			gameConnection.SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
			gameConnection.SetLocalValue( elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
			gameConnection.SetLocalValue( elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );
		}
	}       

	public float ControlPosition( Vector3 target, out float error )
	{
		float currentPosition = Vector3.Dot( body.position - initialPosition, transform.right );
		float maxSetpoint = Mathf.Abs( Vector3.Dot( rangeLimits, transform.right ) );

		float targetSetpoint = Vector3.Dot( target - initialPosition, transform.right );
		//targetSetpoint = Mathf.Clamp( targetSetpoint, -maxSetpoint, maxSetpoint );

		float currentSetpoint = Mathf.Lerp( currentPosition, targetSetpoint, MOVE_INTERVAL );

		error = Mathf.Abs( ( currentSetpoint - currentPosition ) / maxSetpoint );

		if( controlAxis != null && helperEnabled && error > ERROR_THRESHOLD )
		{
			//Debug.Log( "Outside move box: (error: " + error.ToString() + ")" );
			controlAxis.Position = currentSetpoint;
			controlAxis.Velocity = ( targetSetpoint - currentPosition ) / MOVE_INTERVAL;
			controlAxis.Stiffness = 30.0f;
		}

		return targetSetpoint / maxSetpoint;
	}

	void OnEnable()
	{
		controlAxis = Configuration.GetSelectedAxis();
	}

	public void EnableHelper()
	{
		helperEnabled = true;
	}

    void OnTriggerEnter( Collider collider )
    {
        //Debug.Log( "Trigger on " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 60.0f;
    }

    void OnTriggerExit( Collider collider )
    {
        //Debug.Log( "Trigger off " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 60.0f;
    }
}
