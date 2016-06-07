using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class ToAnkleRobot : MonoBehaviour {
	private const int VERTICAL = 0;		// or RIGHT? 	DP - Dorsiflexion/Plantarflexion
	private const int HORIZONTAL = 1;	// or LEFT?		IE - Inversion/Eversion
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

	public bool activeConnection, activeHelper, followBall, elipseSpace, activeDisturber;

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores
	private float playerScore, machineScore, lazyScore;				// Value Scores
	public float lazySpeed, lazyForce;
	
	// Envelope do movimento
	public Vector2 max, min;		// Input for elipse
	public Vector2 bases, origin;	// Elipse's parameters
	public float elipseScale;		// Scale for fitting the moves
	public float squareScale = 1f;

	// Communication with another scripts
	public PlayerController player;
	public EnemyController enemy;
	private Connection connection;
	private Vector2 wallPos;

	// Communication
	public Vector2 input, enemyPos;

	// Control
	private int targetMask;
	public float helperLimit;
	public float helperFade;

	private float fm, fa, dfm, dfa, dt;
	private int eventCounter;

	[Space]

	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float K, D;				// Stiffness and Damping

	private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\AnkleBot\LogFileAnkle - " + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

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
		File.WriteAllText (textFile, "Horizontal\t" +
		                   			 "Vertical" + 
		                   			Environment.NewLine + 
									 "Time\t" +
									 "SqrPos\t\t" +
									 "Pos\t\t" +
									 "FVel\t\t" +
									 "Vel\t\t" +
									 "Torque\t\t" +
			"EventNumber\t" +
			"TorqueVec\t\t" +
			"dTorqueVec\t\t" +
		                   			 "CenterSpring\t\t" +
		                   			 "FreeSpace\t\t" +
		                   			 "K" +
		                   			 "D" +
		                   			Environment.NewLine);
		fm = fa = dt = dfm = dfa = 0f;
		eventCounter = enemy.eventCounter;
	}

	void Update()
	{
		// Update scores
		lazyScore = Time.time - (playerScore + machineScore);
		playerScoreText.text = "Player\n" + playerScore.ToString ("F1");
		machineScoreText.text = "Machine\n" + machineScore.ToString ("F1");
		lazyScoreText.text = "Lazy Time\n" + lazyScore.ToString("F1");
	}

	void FixedUpdate () 
	{
		if (activeConnection)
		{
			input = new Vector2
				(
				connection.ReadStatus(HORIZONTAL, Connection.POSITION),
				connection.ReadStatus(VERTICAL, Connection.POSITION)
				);

			// Move player
			if (elipseSpace)
				wallPos = ElipseToSquare (input);
			else
				wallPos = input * squareScale;
			
			player.SetWalls(wallPos);

			// Player helper
			if (activeHelper)
				PlayerHelper ();

			if (activeDisturber)
				PlayerDisturber ();
			
			if ((new Vector2(connection.ReadStatus(HORIZONTAL, Connection.FORCE),
							 connection.ReadStatus(HORIZONTAL, Connection.FORCE)).magnitude > lazyForce) && activeHelper)
					machineScore += Time.deltaTime;
			else
				if (new Vector2(connection.ReadStatus(HORIZONTAL, Connection.VELOCITY),
			                	connection.ReadStatus(HORIZONTAL, Connection.VELOCITY)).magnitude > lazySpeed)
					playerScore += Time.deltaTime;

			// Follow the ball
			enemyPos = new Vector2 (enemy.enemyBody.position.x, enemy.enemyBody.position.z);

			if (elipseSpace)
				enemyPos = SquareToElipse (enemyPos);
			else
				enemyPos = enemyPos / squareScale / player.boundaryDist;
			
			if (followBall)
				centerSpring = enemyPos;
			
			// Set variables to send to robot
			connection.SetStatus (VERTICAL, centerSpring.y, Connection.CENTERSPRING);
			connection.SetStatus (HORIZONTAL, centerSpring.x, Connection.CENTERSPRING);
			connection.SetStatus (VERTICAL, freeSpace.y, Connection.FREESPACE);
			connection.SetStatus (HORIZONTAL, freeSpace.x, Connection.FREESPACE);

			connection.SetStatus (VERTICAL, K, Connection.STIFF);
			connection.SetStatus (HORIZONTAL, K, Connection.STIFF);
			connection.SetStatus (VERTICAL, D, Connection.DAMP);
			connection.SetStatus (HORIZONTAL, D, Connection.DAMP);

			// Print the all variables
			File.AppendAllText(textFile, 
			                   + Time.time + "\t"
							   + wallPos.x + "\t" 
							   + wallPos.y + "\t");

			for (int j = 0; j < Connection.N_VAR; j++)
				for (int i = 1; i >= 0; i--)
					File.AppendAllText(textFile, connection.ReadStatus(i, j) + "\t");

			File.AppendAllText(textFile, enemy.eventCounter + "\t");

			fm = Mathf.Sqrt (connection.ReadStatus (0, Connection.FORCE) * connection.ReadStatus (0, Connection.FORCE) +
						 	 connection.ReadStatus (1, Connection.FORCE) * connection.ReadStatus (1, Connection.FORCE));
			fa = Mathf.Atan2 (connection.ReadStatus (1, Connection.FORCE),
							  connection.ReadStatus (0, Connection.FORCE));

			File.AppendAllText(textFile, fm + "\t");
			File.AppendAllText(textFile, fa + "\t");

			File.AppendAllText(textFile, (fm - dfm) / (Time.time - dt) + "\t");
			File.AppendAllText(textFile, (fa - dfa) / (Time.time - dt) + "\t");

			dfm = fm;
			dfa = fa;
			dt = Time.time;

			File.AppendAllText(textFile, centerSpring.x + "\t");
			File.AppendAllText(textFile, centerSpring.y + "\t");
			File.AppendAllText(textFile, freeSpace.x + "\t");
			File.AppendAllText(textFile, freeSpace.y + "\t");
			File.AppendAllText(textFile, K + "\t");
			File.AppendAllText(textFile, D + "\t");

			File.AppendAllText(textFile, Environment.NewLine);
			
		} else 
		{
			player.MoveWalls(player.ReadInput());
			input = new Vector2
				(
				player.horizontalWalls [0].position.x/player.boundary/3f,
				player.verticalWalls [0].position.z/player.boundary/3f
				);
		}
		Calibration (input);
	}

	void Calibration(Vector2 position)
	{
		if (max.y < position.y)
			max.y = position.y;
		if (max.x < position.x)
			max.x = position.x;
		if (min.y > position.y)
			min.y = position.y;
		if (min.x > position.x)
			min.x = position.x;
		bases = elipseScale * (max - min) / 2;
		origin = (max + min) / 2;
	}

	void PlayerHelper()
	{
		Vector2 impact, safeArea, track, distance;
		float impactDist;

		impactDist = enemy.FindImpact (targetMask).distance + helperLimit;

		impact = new Vector2 (
			enemy.FindImpact(targetMask).point.x, 
			enemy.FindImpact(targetMask).point.z);

		safeArea = new Vector2 (
			Mathf.Clamp(player.boundary - Mathf.Abs (impact.y), 0f, player.boundary),
			Mathf.Clamp(player.boundary - Mathf.Abs (impact.x), 0f, player.boundary));

		track = new Vector2 (impactDist, impactDist);
		
		distance = (track + safeArea) / enemy.speed * player.speed;

		if (elipseSpace)
		{
			if (helperFade > 1f)
			{
				if ((centerSpring - SquareToElipse (impact)).magnitude < 0.05f)
				{
					centerSpring = SquareToElipse (impact);
					freeSpace = SquareToElipse (distance);
				} else
				{
					helperFade = 0f;
				}
			} else
			{
				centerSpring = Vector2.Lerp (centerSpring, SquareToElipse (impact), helperFade);
				freeSpace = Vector2.Lerp (freeSpace, SquareToElipse (distance), helperFade);
				helperFade += Time.deltaTime;
			}
		} else
		{
			if (helperFade > 1f)
			{
				if ((centerSpring - (impact / squareScale / player.boundaryDist)).magnitude < 0.05f)
				{
					centerSpring = impact / squareScale / player.boundaryDist;
					freeSpace = distance / squareScale / player.boundaryDist;
				} else
				{
					helperFade = 0f;
				}
			} else
			{
				centerSpring = Vector2.Lerp (centerSpring, impact / squareScale / player.boundaryDist, helperFade);
				freeSpace = Vector2.Lerp (freeSpace, distance / squareScale / player.boundaryDist, helperFade);
				helperFade += Time.deltaTime;
			}
		}
	}

	void PlayerDisturber()
	{
		if (helperFade >= 0.2f)
		{
			if (eventCounter != enemy.eventCounter)
			{
				helperFade = 0f;
				eventCounter = enemy.eventCounter;
				Debug.Log ("Event: " + eventCounter + " " + input);
			} 
		}
		else
		{
			centerSpring = Vector2.Lerp (centerSpring, input, helperFade * 5f);
//			if (elipseSpace)
				freeSpace = bases / 3;
//			else
//				freeSpace = new Vector2(
			
			helperFade += Time.deltaTime;
		}
	}

	Vector2 ElipseToSquare(Vector2 elipse)
	{
		float range, r;
		float cosAng, sinAng;
		Vector2 square = Vector2.zero;

						// ATAN2(((X-OX)*BY);((Y-OY)*BX))
		float ang = Mathf.Atan2 ((elipse.y - origin.y) * bases.x, (elipse.x - origin.x)*bases.y);

		cosAng = Mathf.Cos(ang);
		sinAng = Mathf.Sin(ang);

		if (Mathf.Abs(cosAng) < Mathf.Epsilon)
					// (Y - OY)/SIN(T)/BY
			range = ((elipse.y - origin.y)/sinAng/bases.y);
		else
					// (X - OX)/COS(T)/BX
			range = ((elipse.x - origin.x)/cosAng/bases.x);

		if (Mathf.Abs(cosAng) < QUADRANTS)
		{
			r = Mathf.Abs(1f/sinAng);
			square.x = range*r*cosAng;
			square.y = range*Mathf.Sign(sinAng);
		}
		else
		{
			r = Mathf.Abs(1f/cosAng);
			square.x = range*Mathf.Sign(cosAng);
			square.y = range*r*sinAng;
		}
		return (square);
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
		if (!activeConnection)
		{
			activeConnection = true;
			gameObject.AddComponent<Connection> ();
			connection = GetComponent<Connection> ();
		}
	}

	public void Disconnect()
	{
		if (activeConnection)
		{
			activeConnection = false;
			//connection.CloseConnection();
			Destroy (GetComponent <Connection> ());
			connection = null;
		}
	}

}
