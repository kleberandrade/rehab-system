using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(GameClient) ) ]
public class PlayerController : MonoBehaviour 
{
	enum Movable { WALL, BALL };

	public float speed;	// Player Speed
	public EnemyController enemy;		// The ball

	[HideInInspector] public float boundary = 7.25f; 	// Boundary player movement
	[HideInInspector] public float boundaryDist = 10f;	// Distance between boundary
	private float outCut = 2f;			// Gap for helper control

	private int targetMask;

	//private string textFile = "./LogFilePos.txt";

	public Rigidbody[] horizontalWalls;
	public Rigidbody[] verticalWalls;
	public bool controlActive;	// Indicate if helper control is active

    public Robot robot;

	private GameClient gameClient = null;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );
		controlActive = false;

		gameClient = GetComponent<GameClient>();

		foreach( Rigidbody wall in horizontalWalls )
			wall.isKinematic = true;

        if( gameClient.HasRemoteKey( (byte) Movable.BALL, 0 ) ) enemy.enemyBody.isKinematic = true;
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
		UpdateClient();

        MoveWalls( robot.ReadInput() );

//		ControlPosition ();
	}

	// Set the wall's speed
    public void MoveWalls( Vector2 input )
	{
        Debug.Log( "Input: " + input.ToString() );

		foreach( Rigidbody wall in verticalWalls ) 
		{
			//wall.velocity = new Vector3( 0.0f, 0.0f, direction.y * speed );
			// Keep the player inside the boundary
			//wall.position = new Vector3( wall.position.x, 0.0f,	Mathf.Clamp( wall.position.z, -boundary, boundary) );
            wall.MovePosition( new Vector3( wall.position.x, 0.0f,  Mathf.Clamp( input.y * boundary, -boundary, boundary) ) );
		}
	}

	// Set the wall's position
	public void SetWalls( Vector2 position )
	{
		foreach( Rigidbody wall in horizontalWalls )
			wall.position = new Vector3( Mathf.Clamp( position.x * boundary, -boundary, boundary ),	0.0f, wall.position.z );

		foreach( Rigidbody wall in verticalWalls ) 
			wall.position = new Vector3( wall.position.x, 0.0f, Mathf.Clamp( position.y * boundary, -boundary, boundary ) );
	}

	private void UpdateClient()
	{
        // Get remotely controlled object position (z) and set it locally (x)
        foreach( Rigidbody wall in horizontalWalls ) 
            wall.MovePosition( new Vector3( Mathf.Clamp( gameClient.GetRemotePosition( (byte) Movable.WALL, 0 ), -boundary, boundary ), 0.0f, wall.position.z ) );

		// Send locally controlled object positions (z) over network
        foreach( Rigidbody wall in verticalWalls ) 
            gameClient.SetLocalPosition( (byte) Movable.WALL, 0, wall.position.z );

		// Get remotely controlled ball positions (x,z) and set them locally
		if( enemy.enemyBody.isKinematic )
        {
            enemy.enemyBody.MovePosition( new Vector3( Mathf.Clamp( gameClient.GetRemotePosition( (byte) Movable.BALL, 0 ), -boundary, boundary ),
                                          0.0f, Mathf.Clamp( gameClient.GetRemotePosition( (byte) Movable.BALL, 2 ), -boundary, boundary ) ) );
        }
        else
        {
            gameClient.SetLocalPosition( (byte) Movable.BALL, 0, enemy.enemyBody.position.x );
            gameClient.SetLocalPosition( (byte) Movable.BALL, 2, enemy.enemyBody.position.z );
        }
	}

	// Returns a equivalente vector position based on horizontal and vertical walls
	public Vector3 GetPosition()
	{
		Vector3 position = new Vector3( horizontalWalls[ 0 ].position.x, 0.0f, verticalWalls[ 0 ].position.z );
		return position;
	}

	public void ControlPosition()
	{
		RaycastHit enemyImpact = enemy.FindImpact (targetMask);
		Vector3 playerTrack = enemyImpact.point - GetPosition ();

		// Check if the player still able to defend the enemy
		if (playerTrack.magnitude / speed > enemyImpact.distance / enemy.speed) 
		{
			Vector2 control = new Vector2 
				(
				Normalize (OutCut (playerTrack.x, (boundaryDist - Mathf.Abs (enemyImpact.point.z)) / enemy.speed * speed + outCut)),
				Normalize (OutCut (playerTrack.z, (boundaryDist - Mathf.Abs (enemyImpact.point.x)) / enemy.speed * speed + outCut))
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
	}
}
