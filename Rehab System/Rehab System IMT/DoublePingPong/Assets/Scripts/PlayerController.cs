using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class PlayerController : MonoBehaviour 
{
	public float speed;	                                            // Player Speed

    public BoxCollider boundaries;
    private Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );

    public BallController ball;

	private int targetMask;

	//private string textFile = "./LogFilePos.txt";

	public Rigidbody[] horizontalWalls;
	public Rigidbody[] verticalWalls;
	public bool controlActive;	                                    // Indicate if helper control is active

    public Robot robot;

	public GameClient gameClient;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );
		controlActive = false;

        rangeLimits = boundaries.bounds.extents;

		foreach( Rigidbody wall in horizontalWalls )
			wall.isKinematic = true;
	}

	void Start()
	{
		// Start file for record movements
		//if (File.Exists (textFile)) File.Delete (textFile);
		//File.WriteAllText (textFile, "Horizontal\tVertical" + Environment.NewLine);
	}

	void Update()
	{
		// Record movements
		//File.WriteAllText(textFile, horizontalWalls[0].position.x + "\t" + verticalWalls[0].position.z + Environment.NewLine);
	}

	void FixedUpdate()
	{
        MoveWalls( robot.ReadInput() );

		UpdateClient();

//		ControlPosition ();
	}

	// Set the wall's speed
    public void MoveWalls( Vector2 input )
	{
        //Debug.Log( "Input: " + input.ToString() );

		foreach( Rigidbody wall in verticalWalls ) 
		{
            wall.MovePosition( new Vector3( wall.position.x, 0.0f,  Mathf.Clamp( input.y, -1.0f, 1.0f ) * rangeLimits.z ) );
		}
	}

	// Set the wall's position
	/*public void SetWalls( Vector2 position )
	{
		foreach( Rigidbody wall in horizontalWalls )
			wall.position = new Vector3( Mathf.Clamp( position.x * boundary, -boundary, boundary ),	0.0f, wall.position.z );

		foreach( Rigidbody wall in verticalWalls ) 
			wall.position = new Vector3( wall.position.x, 0.0f, Mathf.Clamp( position.y * boundary, -boundary, boundary ) );
	}*/

	private void UpdateClient()
	{
        // Get remotely controlled object position (z) and set it locally (x)
        foreach( Rigidbody wall in horizontalWalls ) 
            wall.MovePosition( new Vector3( Mathf.Clamp( gameClient.GetremoteValue( (byte) Movable.WALL, 0, NetworkValue.POSITION ), -rangeLimits.x, rangeLimits.x ), 0.0f, wall.position.z ) );

		// Send locally controlled object positions (z) over network
        if( robot.Connected ) gameClient.SetLocalValue( (byte) Movable.WALL, 0, NetworkValue.POSITION, verticalWalls[ 0 ].position.z );
	}

	// Returns a equivalente vector position based on horizontal and vertical walls
	public Vector3 GetPosition()
	{
		Vector3 position = new Vector3( horizontalWalls[ 0 ].position.x, 0.0f, verticalWalls[ 0 ].position.z );
		return position;
	}

/*	public void ControlPosition()
	{
		RaycastHit ballImpact = ball.FindImpact( targetMask );
		Vector3 playerTrack = ballImpact.point - GetPosition ();

		// Check if the player still able to defend the ball
		if( playerTrack.magnitude / speed > ballImpact.distance / ball.speed ) 
		{
			Vector2 control = new Vector2 
				(
				Normalize (OutCut (playerTrack.x, (boundaryDist - Mathf.Abs (ballImpact.point.z)) / ball.speed * speed + outCut)),
				Normalize (OutCut (playerTrack.z, (boundaryDist - Mathf.Abs (ballImpact.point.x)) / ball.speed * speed + outCut))
				);
			MoveWalls (control);
//			machineScore += speed * Time.deltaTime * Mathf.Abs(control.magnitude);
			controlActive = true;
		} else
			controlActive = false;
	}
	
	float Normalize( float f )
	{
		if( f > 0.0f ) return 1.0f;
		else if( f < 0.0f ) return -1.0f;
		else return 0.0f;
	}

	// Set the f value to 0 when less then cut
	float OutCut( float f, float cut )
	{
		if( f > cut ) return f;
		else if( f < -cut ) return f;
		else return 0;
	}*/
}
