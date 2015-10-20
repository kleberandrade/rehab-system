using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class PlayerController : MonoBehaviour 
{
	private const int esquerdo = 0;
	private const int direito = 1;
	
	public float speed;
	public Text playerScoreText, machineScoreText, lazyScoreText;
	public float boundary = 7.25f;

	private float outCut = 2f;
	private float boundaryDist = 10f;
	private EnemyController enemy;
	private float playerScore, machineScore, lazyScore;

	private int targetMask;

	private string textFile = @"D:\Users\Thales\Documents\Unity3D\DoublePingPong\LogFilePos.txt";

	[HideInInspector] public Rigidbody[] horizontalWalls;
	[HideInInspector] public Rigidbody[] verticalWalls;
	[HideInInspector] public bool controlActive;

	void Awake()
	{
		GameObject[] hw_aux = GameObject.FindGameObjectsWithTag("Horizontal");
		GameObject[] vw_aux = GameObject.FindGameObjectsWithTag("Vertical");

		horizontalWalls = new Rigidbody[hw_aux.Length];
		verticalWalls = new Rigidbody[vw_aux.Length];

		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i] = hw_aux[i].GetComponent<Rigidbody>();
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i] = vw_aux[i].GetComponent<Rigidbody>();
		}
		targetMask = LayerMask.GetMask ("Target");
		enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();
		controlActive = false;
		
		playerScoreText.text = "Player\n0";
		machineScoreText.text = "Machine\n0";
		lazyScoreText.text = "Lazy Time\n0";
		playerScore = 0;
		machineScore = 0;
		lazyScore = 0;

	}

	void Start ()
	{
		if (File.Exists (textFile)) 
		{
			File.Delete (textFile);
		}
		File.WriteAllText (textFile, "Horizoltal\tVertical" + Environment.NewLine);
	}

	void Update()
	{
		lazyScore = Time.time - (playerScore + machineScore)/speed;
		playerScoreText.text = "Player\n" + playerScore.ToString ("F1");
		machineScoreText.text = "Machine\n" + machineScore.ToString ("F1");
		lazyScoreText.text = "Lazy Time\n" + lazyScore.ToString("F1");
		File.AppendAllText(textFile, horizontalWalls[0].position.x + "\t" + verticalWalls[0].position.z + Environment.NewLine);
	}

	void FixedUpdate()
	{
		MoveWalls(ReadInput ());
//		ControlPosition ();
	}

	public void MoveWalls(Vector2 direction) //(float v, float h)
	{
		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i].velocity = new Vector3 (direction.x*speed, 0f, 0f);
			horizontalWalls[i].position = new Vector3
				(
				Mathf.Clamp (horizontalWalls[i].position.x, -boundary, boundary),
				0.0f,
				horizontalWalls[i].position.z
				);
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i].velocity = new Vector3 (0f, 0f, direction.y*speed);
			verticalWalls[i].position = new Vector3
				(
				verticalWalls[i].position.x,
				0.0f,
				Mathf.Clamp (verticalWalls[i].position.z, -boundary, boundary)
				);
		}
	}

	public void SetWalls (Vector2 position) //(float dv, float dh)
	{
		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i].position = new Vector3
				(
					Mathf.Clamp (position.x*boundary, -boundary, boundary),
					0.0f,
					horizontalWalls[i].position.z
				);
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i].position = new Vector3
				(
					verticalWalls[i].position.x,
					0.0f,
					Mathf.Clamp (position.y*boundary, -boundary, boundary)
				);
		}
	}

	Vector3 GetPosition()
	{
		Vector3 position = new Vector3(horizontalWalls[0].position.x, 0f, verticalWalls[0].position.z);
		return position;
	}

	Vector2 ReadInput()
	{

		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		if (direction != Vector2.zero)
			playerScore += speed * Time.deltaTime;
		
		return direction;
	}

	void ControlPosition()
	{
		RaycastHit enemyImpact = enemy.FindImpact (targetMask);
		Vector3 playerTrack = enemyImpact.point - GetPosition ();

	//	Vector3 distPlayerEnemy = enemy.rigidbody.position - GetPosition ();

		if (playerTrack.magnitude / speed > enemyImpact.distance / enemy.speed) {
			Vector2 control = new Vector2 
				(
				Normalize (OutCut (playerTrack.x, (boundaryDist - Mathf.Abs (enemyImpact.point.z)) / enemy.speed * speed + outCut)),
				Normalize (OutCut (playerTrack.z, (boundaryDist - Mathf.Abs (enemyImpact.point.x)) / enemy.speed * speed + outCut))
				);
			MoveWalls (control);
			machineScore += speed * Time.deltaTime * Mathf.Abs(control.magnitude);
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
