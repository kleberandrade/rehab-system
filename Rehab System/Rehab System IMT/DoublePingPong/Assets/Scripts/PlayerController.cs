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
    public Robot robot;

	void FixedUpdate()
	{
		Vector2 input = robot.ReadInput();

		body.MovePosition( new Vector3( body.position.x, 0.0f, Mathf.Clamp( input.y, -1.0f, 1.0f ) * rangeLimits.z ) );

		//File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + playerBody.position.z.ToString() + Environment.NewLine );

		// Send locally controlled object positions (z) over network
		if( robot.Connected ) gameConnection.SetLocalValue( (byte) Movable.WALL, 0, NetworkValue.POSITION, input.y );
	}       

	public float ControlPosition( Vector3 target, out float error )
	{
        Vector2 setpoint = new Vector2( Mathf.Clamp( target.z, -rangeLimits.z, rangeLimits.z ), 0.0f );

        robot.WriteSetpoint( setpoint );

		error = Mathf.Abs( ( setpoint.x - body.position.z ) / rangeLimits.z );

		return setpoint.x / rangeLimits.z;
	}

    void OnTriggerEnter( Collider collider )
    {
        Debug.Log( "Trigger on " + collider.tag );
        robot.SetImpedance( 1.0f, 0.0f );
    }

    void OnTriggerExit( Collider collider )
    {
        Debug.Log( "Trigger off " + collider.tag );
        robot.SetImpedance( 0.0f, 0.0f );
    }
}
