using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;

using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(InputAxisManager) ) ]
public class Robot : MonoBehaviour 
{
	private const int VERTICAL = 0;		// or RIGHT? 	DP - Dorsiflexion/Plantarflexion
	private const int HORIZONTAL = 1;	// or LEFT?		IE - Inversion/Eversion
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

	public bool activeConnection, activeHelper, followBall;

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores
	private float playerScore, machineScore, lazyScore;				// Value Scores
	public float lazySpeed, lazyForce;
	
	// Envelope do movimento
	public Vector2 max, min;		// Input for elipse
	public Vector2 bases, origin;	// Elipse's parameters
	public float elipseScale;		// Scale for fitting the moves

	// Communication with another scripts
	public PlayerController player;
	public EnemyController enemy;

	//private Connection connection;
	private InputAxis horizontal, vertical;

	// Communication
	public Vector2 input, enemyPos;

	// Control
	private int targetMask;
	public float helperLimit;
	public float helperFade;

	[Space]

	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float K, D;				// Stiffness and Damping

	//private string textFile = "./LogFileAnkle - " + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

	void Awake () 
	{
		targetMask = LayerMask.GetMask ("Target");
		
		playerScoreText.text = "Player\n0";
		machineScoreText.text = "Machine\n0";
		lazyScoreText.text = "Lazy Time\n0";
		playerScore = 0;
		machineScore = 0;
		lazyScore = 0;
	}

	void Start ()
	{
		activeConnection = false;
//		connection = GetComponent<Connection>();
//		File.WriteAllText (textFile, "Horizontal\t" +
//		                   			 "Vertical" + 
//		                   			Environment.NewLine + 
//									 "Time\t" +
//									 "SqrPos\t\t" +
//									 "Pos\t\t" +
//									 "FVel\t\t" +
//									 "Vel\t\t" +
//									 "Torque\t" +
//		                   			 "CenterSpring\t\t" +
//		                   			 "FreeSpace\t\t" +
//		                   			 "K" +
//		                   			 "D" +
//		                   			Environment.NewLine);
	}

	void Update()
	{
		// Update scores
		lazyScore = Time.time - (playerScore + machineScore);
		playerScoreText.text = "Player\n" + playerScore.ToString ("F1");
		machineScoreText.text = "Machine\n" + machineScore.ToString ("F1");
		lazyScoreText.text = "Lazy Time\n" + lazyScore.ToString("F1");
	}

	void FixedUpdate() 
	{
		if( activeConnection )
		{
			input = new Vector2( horizontal.Position, vertical.Position ) * 5.0f;

			// Move player
			//player.SetWalls( ElipseToSquare( input ) );

			// Player helper
			if( activeHelper ) PlayerHelper();

			if( new Vector2( horizontal.Force, horizontal.Force ).magnitude > lazyForce ) machineScore += Time.deltaTime;
			else if( new Vector2( horizontal.Velocity, horizontal.Velocity ).magnitude > lazySpeed ) playerScore += Time.deltaTime;

			// Follow the ball
			enemyPos = new Vector2 (enemy.enemyBody.position.x, enemy.enemyBody.position.z);
			enemyPos = SquareToElipse (enemyPos);
			if( followBall ) centerSpring = enemyPos;

			// Set variables to send to robot
			vertical.Position = centerSpring.y;
			horizontal.Position = centerSpring.x;
			vertical.Velocity = freeSpace.y;
			horizontal.Velocity = freeSpace.x;

			vertical.Stiffness = K;
			horizontal.Stiffness = K;
			vertical.Damping = D;
			horizontal.Damping = D;

			// Print the all variables
//			File.AppendAllText( textFile, + Time.time + "\t"
//			                              + input.x + "\t" 
//			                              + input.y  + "\t" );
//
//			File.AppendAllText( textFile, vertical.Position + "\t" + vertical.Velocity + "\t" + vertical.Force );
//			File.AppendAllText( textFile, horizontal.Position + "\t" + horizontal.Velocity + "\t" + horizontal.Force );
//
//			File.AppendAllText(textFile, centerSpring.x + "\t");
//			File.AppendAllText(textFile, centerSpring.y + "\t");
//			File.AppendAllText(textFile, freeSpace.x + "\t");
//			File.AppendAllText(textFile, freeSpace.y + "\t");
//			File.AppendAllText(textFile, K + "\t");
//			File.AppendAllText(textFile, D + "\t");
//
//			File.AppendAllText(textFile, Environment.NewLine);
			
		} 
        else 
		{
            player.MoveWalls( ReadInput() );
			input = new Vector2( player.horizontalWalls[ 0 ].position.x/player.boundary, player.verticalWalls[ 0 ].position.z/player.boundary );
		}

		Calibration( input );
	}

    public Vector2 ReadInput()
    {
        if( activeConnection ) return input; 

        return Vector2.zero;
    }

	void Calibration( Vector2 position )
	{
		if( max.y < position.y ) max.y = position.y;
		if( max.x < position.x ) max.x = position.x;
		if( min.y > position.y ) min.y = position.y;
		if( min.x > position.x ) min.x = position.x;

		bases = elipseScale * ( max - min ) / 2;
		origin = ( max + min ) / 2;
	}

	void PlayerHelper()
	{
		Vector2 impact, impactBoundary, safeArea, track, distance;
		float impactDist;

		impactDist = enemy.FindImpact( targetMask ).distance + helperLimit;

		impact = new Vector2( enemy.FindImpact( targetMask ).point.x, enemy.FindImpact( targetMask ).point.z );

		impactBoundary = new Vector2( Mathf.Clamp( impact.x, -player.boundary, player.boundary ), Mathf.Clamp( impact.y, -player.boundary, player.boundary ) );
		
		safeArea = new Vector2(	player.boundary - Mathf.Abs( impactBoundary.y ), player.boundary - Mathf.Abs( impactBoundary.x ) );
		
//		track = new Vector2	(
//			Mathf.Max( Mathf.Abs(enemy.enemyBody.position.x - impact.x), helperLimit),
//			Mathf.Max( Mathf.Abs(enemy.enemyBody.position.z - impact.y), helperLimit));

		track = new Vector2( impactDist, impactDist );
		
		distance = ( track + safeArea ) / enemy.speed * player.speed;
		
		if( helperFade >= 1.0f )
		{
			if( ( centerSpring - SquareToElipse( impact ) ).magnitude < 0.05f )
			{
				centerSpring = SquareToElipse( impact );
				freeSpace = SquareToElipse( distance );
			}
			else
			{
				helperFade = 0.0f;
			}
		}
		else
		{
			centerSpring = Vector2.Lerp( centerSpring, SquareToElipse( impact ), helperFade );
			freeSpace = Vector2.Lerp( freeSpace, SquareToElipse( distance ), helperFade );
			helperFade += Time.deltaTime;
		}
	}

	Vector2 ElipseToSquare( Vector2 elipse )
	{
		float range, r;
		float cosAng, sinAng;
		Vector2 square = Vector2.zero;
        			
        float ang = Mathf.Atan2( ( elipse.y - origin.y ) * bases.x, ( elipse.x - origin.x ) * bases.y );    // ATAN2(((X-OX)*BY);((Y-OY)*BX))

		cosAng = Mathf.Cos( ang );
		sinAng = Mathf.Sin( ang );
         
        if( Mathf.Abs( cosAng ) < Mathf.Epsilon ) range = ( elipse.y - origin.y ) / sinAng / bases.y;   // (Y - OY)/SIN(T)/BY
        else range = ( elipse.x - origin.x ) / cosAng / bases.x;                                        // (X - OX)/COS(T)/BX
					
		if( Mathf.Abs( cosAng ) < QUADRANTS )
		{
			r = Mathf.Abs( 1.0f / sinAng );
			square.x = range * r * cosAng;
			square.y = range * Mathf.Sign( sinAng );
		}
		else
		{
			r = Mathf.Abs( 1.0f / cosAng );
			square.x = range * Mathf.Sign( cosAng );
			square.y = range * r * sinAng;
		}

		return square;
	}

	
	Vector2 SquareToElipse(Vector2 square)
	{
		float range;
		float cosAng, sinAng;
		Vector2 elipse = Vector2.zero;
		
		// ATAN2(((X-OX)*BY);((Y-OY)*BX))
		float ang = Mathf.Atan2 (square.y, square.x);

		cosAng = Mathf.Cos(ang);
		sinAng = Mathf.Sin(ang);

		range = Mathf.Abs(square.x) > Mathf.Abs(square.y) ?
			Mathf.Abs(square.x / player.boundaryDist) :
			Mathf.Abs(square.y / player.boundaryDist);

		elipse.x = origin.x + range * cosAng * bases.x; // / elipseScale;
		elipse.y = origin.y + range * sinAng * bases.y; // / elipseScale;
		return (elipse);
	}

	public void Connect()
	{
		activeConnection = true;

        horizontal = GetComponent<InputAxisManager>().GetAxis( "1" );
        vertical = GetComponent<InputAxisManager>().GetAxis( "0" );

        if( horizontal == null ) horizontal = GetComponent<InputAxisManager>().GetAxis( "Horizontal", InputAxisType.Keyboard );
        if( vertical == null ) vertical = GetComponent<InputAxisManager>().GetAxis( "Vertical", InputAxisType.Keyboard );
	}

	public void Disconnect()
	{
		activeConnection = false;
	}

}
