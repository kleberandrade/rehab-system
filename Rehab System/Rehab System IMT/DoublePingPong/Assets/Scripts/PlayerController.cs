using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class PlayerController : MonoBehaviour 
{
	public float speed;	// Player Speed
	public EnemyController enemy;		// The ball

	[HideInInspector] public float boundary = 7.25f; 	// Boundary player movement
	[HideInInspector] public float boundaryDist = 10f;	// Distance between boundary
	private float outCut = 2f;			// Gap for helper control

	private int targetMask;

	private string textFile = "./LogFilePos.txt";

	public Rigidbody[] horizontalWalls;
	public Rigidbody[] verticalWalls;
	public bool controlActive;	// Indicate if helper control is active

	private GameConnection gameConnection = null;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );
		controlActive = false;

		string networkRole = PlayerPrefs.GetString( "Network Role", "client" );

		// Network update coroutines
		if( networkRole == "server" )
		{
			gameConnection = gameObject.AddComponent<GameServer>();

			foreach( Rigidbody wall in horizontalWalls )
				wall.isKinematic = true;

			StartCoroutine( UpdateServer() );
		} 
		else if( networkRole == "client" )
		{
			gameConnection = gameObject.AddComponent<GameClient>();

			foreach( Rigidbody wall in horizontalWalls )
				wall.isKinematic = true;

			enemy.enemyBody.isKinematic = true;

			StartCoroutine( UpdateClient() );
		} 			
	}

	void Start()
	{
		// Start file for record movements
		if (File.Exists (textFile)) File.Delete (textFile);
		File.WriteAllText (textFile, "Horizoltal\tVertical" + Environment.NewLine);
	}

	void Update()
	{
		// Record movements
		File.WriteAllText(textFile, horizontalWalls[0].position.x + "\t" + verticalWalls[0].position.z + Environment.NewLine);
	}

	void FixedUpdate()
	{
		MoveWalls( ReadInput() );

//		ControlPosition ();
	}

	// Set the wall's speed
	public void MoveWalls( Vector2 direction )
	{
		// Control horizontal walls if on local match
		if( gameConnection == null )
		{
			foreach( Rigidbody wall in horizontalWalls )
			{
				wall.velocity = new Vector3( direction.x * speed, 0f, 0f );
				// Keep the player inside the boundary
				wall.position = new Vector3( Mathf.Clamp( wall.position.x, -boundary, boundary ), 0.0f, wall.position.z );
			}
		}

		foreach( Rigidbody wall in verticalWalls ) 
		{
			wall.velocity = new Vector3( 0f, 0f, direction.y * speed );
			// Keep the player inside the boundary
			wall.position = new Vector3( wall.position.x, 0.0f,	Mathf.Clamp( wall.position.z, -boundary, boundary) );
		}
	}

	// Set the wall's position
	public void SetWalls( Vector2 position )
	{
		if( gameConnection == null )
		{
			foreach( Rigidbody wall in horizontalWalls )
				wall.position = new Vector3( Mathf.Clamp( position.x * boundary, -boundary, boundary ),	0.0f, wall.position.z );
		}

		foreach( Rigidbody wall in verticalWalls ) 
			wall.position = new Vector3( wall.position.x, 0.0f, Mathf.Clamp( position.y * boundary, -boundary, boundary ) );
	}

	public IEnumerator UpdateClient()
	{
		while( Application.isPlaying )
		{
			for( int i = 0; i < horizontalWalls.Length; i++ ) 
			{
				// Get remotely controlled object position (z) and set it locally (x)
				horizontalWalls[i].position = new Vector3( Mathf.Clamp( gameConnection.GetRemotePosition( (byte) i, 0 ), -boundary, boundary ),
					0.0f, horizontalWalls[i].position.z );
			}

			// Send locally controlled object positions (z) over network
			for( int i = 0; i < verticalWalls.Length; i++ ) 
				gameConnection.SetLocalPosition( (byte) i, 0, verticalWalls[i].position.z );

			// Get remotely controlled ball positions (x,z) and set them locally
			enemy.enemyBody.position = new Vector3(	Mathf.Clamp( gameConnection.GetRemotePosition( (byte) 2, 0 ), -boundary, boundary ),
					                                             0.0f, Mathf.Clamp( gameConnection.GetRemotePosition( (byte) 2, 2 ), -boundary, boundary ) );

			yield return new WaitForFixedUpdate();
		}
	}

	public IEnumerator UpdateServer()
	{
		while( Application.isPlaying )
		{ 
			for( int i = 0; i < horizontalWalls.Length; i++ ) 
			{
				// Get remotely controlled object position (z) and set it locally (x)
				horizontalWalls[i].position = new Vector3( Mathf.Clamp( gameConnection.GetRemotePosition( (byte) i, 0 ), -boundary, boundary ),
					0.0f, horizontalWalls[i].position.z );
			}

			// Send locally controlled object positions (z) over network
			for( int i = 0; i < verticalWalls.Length; i++ ) 
				gameConnection.SetLocalPosition( (byte) i, 0, verticalWalls[i].position.z );

			// Send locally controlled object positions (x,z) over network
			gameConnection.SetLocalPosition( 2, 0, enemy.enemyBody.position.x );
			gameConnection.SetLocalPosition( 2, 2, enemy.enemyBody.position.z );

			yield return new WaitForFixedUpdate();
		}
	}

	// Returns a equivalente vector position based on horizontal and vertical walls
	public Vector3 GetPosition()
	{
		Vector3 position = new Vector3(horizontalWalls[0].position.x, 0f, verticalWalls[0].position.z);
		return position;
	}

	// Read movemente input
	public Vector2 ReadInput()
	{
		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

//		if (direction != Vector2.zero)
//			playerScore += speed * Time.deltaTime;
		
		return direction;
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
	
	float Normalize(float f)
	{
		if (f > 0f)
			return 1f;
		else if (f < 0f)
			return -1f;
		else return 0f;
	}

	// Set the f value to 0 when less then cut
	float OutCut(float f, float cut)
	{
		if (f > cut)
			return f;
		else if (f < -cut)
			return f;
		else
			return 0;

	}
}
