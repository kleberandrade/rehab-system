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
	const float MOVE_INTERVAL = 1.0f;

	private InputAxis controlAxis = null;
	private bool connected = false, helperEnabled = false;

	void FixedUpdate()
	{
		if( controlAxis != null )
		{
			if( connected )
			{
				float input = controlAxis.NormalizedPosition;
				Debug.Log( "Input position: " + (transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x )).ToString() );
				body.MovePosition( transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x ) + initialPosition );

				//File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + playerBody.position.z.ToString() + Environment.NewLine );

				// Send locally controlled object position over network
				gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
				gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
				gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
				gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );
			} 
			else
			{
				if( controlAxis.Position > controlAxis.MaxValue ) controlAxis.MaxValue = controlAxis.Position;
				else if( controlAxis.Position < controlAxis.MinValue ) controlAxis.MinValue = controlAxis.Position;
			}
		}
	}       

	public float ControlPosition( Vector3 target, out float error )
	{
		// FIX THAT !!!!

		float targetPosition = Mathf.Clamp( target.x, -rangeLimits.x, rangeLimits.x );
		float setpoint = Mathf.Lerp( body.position.x, targetPosition, MOVE_INTERVAL );

		if( controlAxis != null && helperEnabled )
		{
			controlAxis.Position = setpoint;
			controlAxis.Velocity = ( targetPosition - body.position.x ) / MOVE_INTERVAL;
		}

		error = Mathf.Abs( ( setpoint - body.position.x ) / rangeLimits.x );

		return targetPosition / rangeLimits.x;
	}

	void OnEnable()
	{
		controlAxis = AxisSelector.GetSelectedAxis();
	}

	public void Connect()
	{
		connected = true;
	}

	public void EnableHelper()
	{
		helperEnabled = true;
	}

    void OnTriggerEnter( Collider collider )
    {
        //Debug.Log( "Trigger on " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 1.0f;
    }

    void OnTriggerExit( Collider collider )
    {
        //Debug.Log( "Trigger off " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 0.0f;
    }
}
