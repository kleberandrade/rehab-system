using UnityEngine;
using System.Globalization;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class ToAnkleRobot : MonoBehaviour {

	private const int VERTICAL = 0;		// or RIGHT?
	private const int HORIZONTAL = 1;	// or LEFT?
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

	public bool activeConnection, activeHelper, followBall;

	// Envelope do movimento
	public Vector2 max, min;		// Input for elipse
	public Vector2 bases, origin;	// Elipse's parameters
	public float elipseScale;		// Scale for fitting the moves

	// Communication with another scripts
	public PlayerController player;
	public EnemyController enemy;
	private Connection connection;

	// Communication
	public Vector2 input, enemyPos;
	private float timeFree;

	// Control
	private int targetMask;
	public float helperLimit;
	public float helperFade;

	[Space]

	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float K, D;				// Stiffness and Damping

	private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\AnkleBot\LogFileAnkle - " + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

	void Awake () 
	{
		connection = GetComponent<Connection>();
		File.WriteAllText (textFile, "Horizontal\t" +
		                   			 "Vertical" + 
		                   			Environment.NewLine + 
									 "Time\t" +
									 "SqrPos\t\t" +
									 "Pos\t\t" +
									 "FVel\t\t" +
									 "Vel\t\t" +
									 "Torque" +
		                   			Environment.NewLine);
		targetMask = LayerMask.GetMask ("Target");
		timeFree = 0f;
	}

//	void Start()
//	{
//	}

	void FixedUpdate () 
	{
		if (activeConnection)
		{
			input = new Vector2
				(
				connection.ReadStatus(HORIZONTAL, Connection.POSITION),
				connection.ReadStatus(VERTICAL, Connection.POSITION)
				);
			File.AppendAllText(textFile, 
			                   + Time.time + "\t"
			                   + input.x + "\t" 
			                   + input.y  + "\t");

			for (int j = 0; j < Connection.N_VAR; j++)
				for (int i = 1; i >= 0; i--)
					File.AppendAllText(textFile, connection.ReadStatus(i, j) + "\t");
			File.AppendAllText(textFile, Environment.NewLine);

			player.SetWalls(ElipseToSquare(input));

			connection.SetStatus (VERTICAL, centerSpring.y, Connection.POSITION);
			connection.SetStatus (HORIZONTAL, centerSpring.x, Connection.POSITION);
			connection.SetStatus (VERTICAL, freeSpace.y, Connection.VELOCITY);
			connection.SetStatus (HORIZONTAL, freeSpace.x, Connection.VELOCITY);

			connection.SetStatus (VERTICAL, K, Connection.STIFF);
			connection.SetStatus (HORIZONTAL, K, Connection.STIFF);
			connection.SetStatus (VERTICAL, D, Connection.DAMP);
			connection.SetStatus (HORIZONTAL, D, Connection.DAMP);
		} else 
		{
			player.MoveWalls(player.ReadInput());
			input = new Vector2
				(
				player.horizontalWalls [0].position.x/player.boundary,
				player.verticalWalls [0].position.z/player.boundary
				);
		}
		Calibration (input);
		enemyPos = new Vector2 (enemy.enemyBody.position.x, enemy.enemyBody.position.z);
		enemyPos = SquareToElipse (enemyPos);

		if (activeHelper)
			PlayerHelper ();

		if (followBall)
			centerSpring = enemyPos;
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
		Vector2 impact, impactBoundary, safeArea, track, distance;
		float impactDist;

		impactDist = enemy.FindImpact (targetMask).distance + helperLimit;

		impact = new Vector2 (
			enemy.FindImpact(targetMask).point.x, 
			enemy.FindImpact(targetMask).point.z);

		impactBoundary = new Vector2 (
			Mathf.Clamp(impact.x, -player.boundary, player.boundary), 
			Mathf.Clamp(impact.y, -player.boundary, player.boundary));
		
		safeArea = new Vector2 (
			player.boundary - Mathf.Abs (impactBoundary.y),
			player.boundary - Mathf.Abs (impactBoundary.x));
		
//		track = new Vector2	(
//			Mathf.Max( Mathf.Abs(enemy.enemyBody.position.x - impact.x), helperLimit),
//			Mathf.Max( Mathf.Abs(enemy.enemyBody.position.z - impact.y), helperLimit));

		track = new Vector2 (impactDist, impactDist);
		
		distance = (track + safeArea) / enemy.speed * player.speed;
		
		if (helperFade >= 1f)
		{
			if ((centerSpring - SquareToElipse (impact)).magnitude < 0.05f)
			{
				centerSpring = SquareToElipse (impact);
				freeSpace = SquareToElipse (distance);
			}
			else
			{
				helperFade = 0f;
			}
		}
		else
		{
			centerSpring = Vector2.Lerp( centerSpring, SquareToElipse (impact), helperFade);
			freeSpace = Vector2.Lerp( freeSpace, SquareToElipse (distance), helperFade);
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

}
