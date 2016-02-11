using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class PlayerController : MonoBehaviour 
{
    public int elementID;

	public Collider boundaries;
    private Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );

    public BallController ball;

	private int targetMask;

    private string textFile;

    private Rigidbody playerBody;
    private BoxCollider playerCollider;

    public Robot robot;

	public GameClient gameClient;

	void Start()
	{
        targetMask = LayerMask.GetMask( "Target" );

        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();

        rangeLimits = boundaries.bounds.extents - playerCollider.bounds.extents;

		// Start file for record movements
        textFile = "./LogFilePlayer" + GetInstanceID().ToString() + ".txt";
		if( File.Exists( textFile ) ) File.Delete( textFile );
	}

	void FixedUpdate()
	{
        MoveWalls( robot.ReadInput() );

		ControlPosition();
	}

	// Set the wall's speed
    public void MoveWalls( Vector2 input )
	{
        playerBody.MovePosition( new Vector3( playerBody.position.x, 0.0f, Mathf.Clamp( input.y, -1.0f, 1.0f ) * rangeLimits.z ) );

        File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + playerBody.position.z.ToString() + Environment.NewLine );

        // Send locally controlled object positions (z) over network
        if( robot.Connected ) gameClient.SetLocalValue( (byte) Movable.WALL, 0, NetworkValue.POSITION, input.y );
	}        

	public void ControlPosition()
	{
        Vector3 impactPoint = ball.FindImpactPoint( targetMask );

        Vector2 setpoint = new Vector2( Mathf.Clamp( impactPoint.z, -rangeLimits.z, rangeLimits.z ), 0.0f );

        robot.WriteSetpoint( setpoint );
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
