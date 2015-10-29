using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class ToAnkleRobot : MonoBehaviour {

	private const int VERTICAL = 0;		// or RIGHT?
	private const int HORIZONTAL = 1;	// or LEFT?
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

	public bool activeConnection, activeHelper;

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

	[Space]

	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float K, D;				// Stiffness and Damping

	private string textFile = @"D:\Users\Thales\Documents\Unity3D\DoublePingPong\LogFileAnkle.txt";

	void Awake () 
	{
		connection = GetComponent<Connection>();
		File.WriteAllText (textFile, "Horizoltal\tVertical\tH_Elipse\tV_Elipse" + Environment.NewLine);
		timeFree = -100f;
	}

//	void Start()
//	{
//	}

	void Update () 
	{
		if (activeConnection)
		{
			input = new Vector2
				(
				connection.ReadStatus(HORIZONTAL, Connection.POSITION),
				connection.ReadStatus(VERTICAL, Connection.POSITION)
				);
			File.AppendAllText(textFile, input.x + "\t" + input.y  + "\t"
			                   + ElipseToSquare(input).x + "\t" + ElipseToSquare(input).y + Environment.NewLine);
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
	//		centerSpring = SquareToElipse(new Vector2(enemy.enemyBody.position.x, enemy.enemyBody.position.z));
		}
		if (activeHelper)
			PlayerHelper ();
//		else 
//		{ 
//			K = 0;
//			D = 0;
//		}
		Calibration (input);
		enemyPos = new Vector2 (enemy.enemyBody.position.x, enemy.enemyBody.position.z);
		enemyPos = SquareToElipse (enemyPos);
//		player.SetWalls(ElipseToSquare(input));
//		}
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
		RaycastHit enemyImpact = enemy.FindImpact (player.targetMask);
	//	Vector3 playerTrack = enemyImpact.point - player.GetPosition ();
		float distance = enemyImpact.distance / enemy.speed * player.speed;

		timeFree += Time.deltaTime;
		if (timeFree > 0f)
		{
			centerSpring = SquareToElipse (new Vector2 (enemyImpact.point.x, enemyImpact.point.z));
			if (distance > 0.5f)
				freeSpace = SquareToElipse (new Vector2 (distance, distance));
			else if (distance < 0.05f)
				{
				freeSpace = SquareToElipse (new Vector2 (player.boundaryDist, player.boundaryDist));
				timeFree = -100f;
				}
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

		elipse.x = origin.x + range * cosAng * bases.x / elipseScale;
		elipse.y = origin.y + range * sinAng * bases.y / elipseScale;
		return (elipse);
	}

}
//		if (player.controlActive) 
//		{
//			Vector3 position = enemy.enemyTrack.point / player.boundary;
//			connection.SetStatus (ESQUERDO, position.z * desloc_max + position.x * ang_max / 2, Connection.POSITION);
//			connection.SetStatus (DIREITO, position.z * desloc_max - position.x * ang_max / 2, Connection.POSITION);
//
//			Vector3 velocity = new Vector3(
//				player.horizontalWalls[0].GetComponent<Rigidbody> ().velocity.x / player.speed,
//				0,
//				player.verticalWalls[0].GetComponent<Rigidbody> ().velocity.z / player.speed
//				);
//			connection.SetStatus (ESQUERDO, velocity.z * speed_max + velocity.x * rot_max / 2, Connection.VELOCITY);
//			connection.SetStatus (DIREITO, velocity.z * speed_max - velocity.x * rot_max / 2, Connection.VELOCITY);
//
//			connection.SetStatus (ESQUERDO, K, Connection.STIFF);
//			connection.SetStatus (DIREITO, K, Connection.STIFF);
//			connection.SetStatus (ESQUERDO, D, Connection.DAMP);
//			connection.SetStatus (DIREITO, D, Connection.DAMP);
//		} else
//			{
//			connection.SetStatus (ESQUERDO, player.v_aux * speed_max + player.h_aux * rot_max / 2, Connection.VELOCITY);
//			connection.SetStatus (DIREITO, player.v_aux * speed_max - player.h_aux * rot_max / 2, Connection.VELOCITY);
//			connection.ClearMask ();
//
////			h = (connection.ReadStatus(ESQUERDO, Connection.VELOCITY) - connection.ReadStatus(DIREITO, Connection.VELOCITY))/rot_max;
////			v = (connection.ReadStatus(ESQUERDO, Connection.VELOCITY) + connection.ReadStatus(DIREITO, Connection.VELOCITY))/(rot_max*2);
////			player.MoveWalls(v, h);
//			}
//	}
//}
