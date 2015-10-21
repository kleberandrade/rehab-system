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
	private float outCut = 2f;			// Gap for helper control
	private float boundaryDist = 10f;	// Distance between boundary

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores
	private float playerScore, machineScore, lazyScore;				// Value Scores

	private int targetMask;

	private string textFile = @"D:\Users\Thales\Documents\Unity3D\DoublePingPong\LogFilePos.txt";

	public Rigidbody[] horizontalWalls;
	public Rigidbody[] verticalWalls;
	public bool controlActive;	// Indicate if helper control is active

	void Awake()
	{
		targetMask = LayerMask.GetMask ("Target");
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
		// Start file for record movements
		if (File.Exists (textFile)) File.Delete (textFile);
		File.WriteAllText (textFile, "Horizoltal\tVertical" + Environment.NewLine);
	}

	void Update()
	{
		// Update scores
		lazyScore = Time.time - (playerScore + machineScore)/speed;
		playerScoreText.text = "Player\n" + playerScore.ToString ("F1");
		machineScoreText.text = "Machine\n" + machineScore.ToString ("F1");
		lazyScoreText.text = "Lazy Time\n" + lazyScore.ToString("F1");

		// Record movements
		File.AppendAllText(textFile, horizontalWalls[0].position.x + "\t" + verticalWalls[0].position.z + Environment.NewLine);
	}

	void FixedUpdate()
	{
		MoveWalls(ReadInput ());
//		ControlPosition ();
	}

	// Set the wall's speed
	public void MoveWalls(Vector2 direction)
	{
		for (int i = 0; i < horizontalWalls.Length; i++) 
		{
			horizontalWalls[i].velocity = new Vector3 (direction.x*speed, 0f, 0f);
			horizontalWalls[i].position = new Vector3
				(
				Mathf.Clamp (horizontalWalls[i].position.x, -boundary, boundary), // Keep the player inside the boundary
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
				Mathf.Clamp (verticalWalls[i].position.z, -boundary, boundary) // Keep the player inside the boundary
				);
		}
	}

	// Set the wall's position
	public void SetWalls (Vector2 position)
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

		if (direction != Vector2.zero)
			playerScore += speed * Time.deltaTime;
		
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
