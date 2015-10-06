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

	[HideInInspector] public GameObject[] horizontalWalls;
	[HideInInspector] public GameObject[] verticalWalls;
	[HideInInspector] public bool controlActive;

	void Start ()
	{
		GameObject[] hw_aux = GameObject.FindGameObjectsWithTag("Horizontal");
		GameObject[] vw_aux = GameObject.FindGameObjectsWithTag("Vertical");
		enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();
		controlActive = false;

		horizontalWalls = new GameObject[hw_aux.Length];
		verticalWalls = new GameObject[vw_aux.Length];
		targetMask = LayerMask.GetMask ("Target");

		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i] = hw_aux[i];
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i] = vw_aux[i];
		}

		playerScoreText.text = "Player\n0";
		machineScoreText.text = "Machine\n0";
		lazyScoreText.text = "Lazy Time\n0";
		playerScore = 0;
		machineScore = 0;
		lazyScore = 0;

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
		File.AppendAllText(textFile, horizontalWalls[0].GetComponent<Rigidbody>().position.x + "\t" + verticalWalls[0].GetComponent<Rigidbody>().position.z + Environment.NewLine);
	}

	void FixedUpdate()
	{
		ReadInput ();
//		ControlPosition ();
	}

	public void MoveWalls(Vector2 direction) //(float v, float h)
	{
		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i].GetComponent<Rigidbody>().velocity = new Vector3 (direction.x*speed, 0f, 0f);
			horizontalWalls[i].GetComponent<Rigidbody>().position = new Vector3
				(
				Mathf.Clamp (horizontalWalls[i].GetComponent<Rigidbody>().position.x, -boundary, boundary),
				0.0f,
				horizontalWalls[i].GetComponent<Rigidbody>().position.z
				);
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i].GetComponent<Rigidbody>().velocity = new Vector3 (0f, 0f, direction.y*speed);
			verticalWalls[i].GetComponent<Rigidbody>().position = new Vector3
				(
				verticalWalls[i].GetComponent<Rigidbody>().position.x,
				0.0f,
				Mathf.Clamp (verticalWalls[i].GetComponent<Rigidbody>().position.z, -boundary, boundary)
				);
		}
	}

	public void SetWalls (Vector2 position) //(float dv, float dh)
	{
		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i].GetComponent<Rigidbody>().position = new Vector3
				(
					Mathf.Clamp (position.x*boundary, -boundary, boundary),
					0.0f,
					horizontalWalls[i].GetComponent<Rigidbody>().position.z
				);
		}
		for (int i = 0; i < verticalWalls.Length; i++) 
		{
			verticalWalls[i].GetComponent<Rigidbody>().position = new Vector3
				(
					verticalWalls[i].GetComponent<Rigidbody>().position.x,
					0.0f,
					Mathf.Clamp (position.y*boundary, -boundary, boundary)
				);
		}
	}

	Vector3 GetPosition()
	{
		Vector3 position = new Vector3(horizontalWalls[0].GetComponent<Rigidbody>().position.x, 0f, verticalWalls[0].GetComponent<Rigidbody>().position.z);
		return position;
	}

	void ReadInput()
	{

		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		if (direction != Vector2.zero)
			playerScore += speed * Time.deltaTime;
		
		MoveWalls (direction);
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
