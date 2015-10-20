using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class ToAnkleRobot : MonoBehaviour {


//	private const int ESQUERDO = 0;
//	private const int DIREITO = 1;

	private const int VERTICAL = 0;		// ou ESQUERDO?
	private const int HORIZONTAL = 1;	// ou DIREITO?
	private const float QUADRANTES = 0.70710678118654752440084436210485f;

//	public float desloc_max, speed_max;
//	public float ang_max, rot_max;

	// Envelope do movimento
	public Vector2 max, min;		// Dados de entrada da Elipse
	public Vector2 bases, origin;	// Parametros da Elipse
	public float elipseScale;

	public float K, D;

	private PlayerController player;
	private EnemyController enemy;
	private Connection connection;
	private Vector2 desloc;
//	private float dh, dv;

	public GameObject point, background, space;
	private RectTransform r_point, r_background, r_space;
//	public float px, py, sx, sy;

	public Vector2 input;

	private string textFile = @"D:\Users\Thales\Documents\Unity3D\DoublePingPong\LogFileAnkle.txt";

	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();
		connection = GameObject.FindGameObjectWithTag("Connection").GetComponent<Connection>();
		File.WriteAllText (textFile, "Horizoltal\tVertical\tH_Elipse\tV_Elipse" + Environment.NewLine);

//		px = point.GetComponent<RectTransform> ().anchoredPosition.x;
//		py = point.GetComponent<RectTransform> ().anchoredPosition.y;
//		sx = back.GetComponent<RectTransform> ().localScale.x;
//		sy = back.GetComponent<RectTransform> ().localScale.y;


		//scalePoint = back.GetComponentInParent<RectTransform>().sizeDelta.x;
	}

	void Start()
	{
		r_point = point.GetComponent<RectTransform> ();
		r_background = background.GetComponent<RectTransform> ();
		r_space = space.GetComponent<RectTransform> ();
	}

	 

	void Update () 
	{
//		if (player.controlActive)
//		{
			Vector3 position = enemy.enemyTrack.point / player.boundary;
		//	connection.SetStatus (VERTICAL, position.z * dv_max, Connection.POSITION);
		//	connection.SetStatus (HORIZONTAL, position.x * dh_max, Connection.POSITION);

			Vector3 velocity = new Vector3(
								player.horizontalWalls[0].velocity.x / player.speed,
								0,
								player.verticalWalls[0].velocity.z / player.speed
								);
		//	connection.SetStatus (VERTICAL, velocity.z * sv_max, Connection.VELOCITY);
		//	connection.SetStatus (HORIZONTAL, velocity.x * sh_max, Connection.VELOCITY);

			connection.SetStatus (VERTICAL, K, Connection.STIFF);
			connection.SetStatus (HORIZONTAL, K, Connection.STIFF);
			connection.SetStatus (VERTICAL, D, Connection.DAMP);
			connection.SetStatus (HORIZONTAL, D, Connection.DAMP);
//		}
//		else
//		{

		input = new Vector2
			(
//			connection.ReadStatus(HORIZONTAL, Connection.POSITION),
//			connection.ReadStatus(VERTICAL, Connection.POSITION)
			player.horizontalWalls [0].position.x/10f,
			player.verticalWalls [0].position.z/10f
			);

		File.AppendAllText(textFile, input.x + "\t" + input.y  + "\t"
		                   + ElipseToSquare(input).x + "\t" + ElipseToSquare(input).y + Environment.NewLine);

		Calibration (input);
//		player.SetWalls(ElipseToSquare(input));
//		}
	
		r_point.anchoredPosition = input*elipseScale*r_space.rect.width;
		r_background.anchoredPosition = origin;
		r_background.localScale = bases;
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

		if (Mathf.Abs(cosAng) < QUADRANTES)
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
