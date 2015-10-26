using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed;	// Enemy speed
	public float timeDelay = 0.5f;	// Delay for creating another "pickup"

	public GameObject pickUp;		// Pickup will appear on the next impact point
	public RaycastHit enemyTrack;

	private int pickUpMask;			// Mask for the playe where "pickup" will appear

	private float pickUpTimeCount;	// Delay for next "pickup" placing
	private float multiHitCheck;	// Variable auxiliar for checking multiple impact

	[HideInInspector] public Rigidbody enemyBody;		// Enemy rigid body

	void Awake(){
		enemyBody = GetComponent<Rigidbody> ();
		pickUpMask = LayerMask.GetMask ("PickUp");
	}

	void Start () 
	{
		enemyBody.velocity = RandVectOnGround();	// Inicialize with a random velocity
		multiHitCheck = Time.time;
		pickUpTimeCount = 0f;
	}

	void FixedUpdate()
	{
		float vy = enemyBody.velocity.y;
		enemyBody.velocity = enemyBody.velocity.normalized * speed;	// Even setting drag to zero, body still losing energy, so this keep the speed
		enemyBody.velocity = new Vector3(enemyBody.velocity.x, vy, enemyBody.velocity.z);

		// Counting delay for new "pickup" 
		pickUpTimeCount += Time.deltaTime;	
		if (pickUpTimeCount > timeDelay)	
		{
			enemyTrack = FindImpact(pickUpMask);
			Instantiate(pickUp, enemyTrack.point, Quaternion.identity);	// Instantiate a new "pickup" 
			pickUpTimeCount = -100f;

		}

		// Alternative enemy control for testing
		MoveEnemy();
	}

	void HitWall(string wall)
	{
		if (Mathf.Abs(multiHitCheck - Time.time) <= Mathf.Epsilon)
		{
			enemyBody.velocity = new Vector3(-enemyBody.position.x, 0f, -enemyBody.position.z);
			return;
		}
		switch (wall)
		{
		case "Vertical":
			enemyBody.velocity = new Vector3(-enemyBody.velocity.x, 0f, Random.Range(-speed, speed));
			break;
		case "Horizontal": 
			enemyBody.velocity = new Vector3(Random.Range(-speed, speed), 0f, -enemyBody.velocity.z);
			break;
		case "Tower":
			enemyBody.velocity = new Vector3(-enemyBody.velocity.x, 0f, -enemyBody.velocity.z);
			break;
		}
		multiHitCheck = Time.time;
		enemyBody.velocity = enemyBody.velocity.normalized * speed;
		pickUpTimeCount = 0f;
	}

	void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag) 
		{
		case "Vertical":
		case "Horizontal":
		case "Tower":
			HitWall(other.gameObject.tag); 
			break;
		case "PickUp":
			Destroy(other.gameObject);
			break;
		case "Sky":
			enemyBody.position = new Vector3(0f, 23.5f, 0f); // Vector3.zero;
			enemyBody.velocity = RandVectOnGround();
			break;
		}
	}

	public RaycastHit FindImpact(int mask)
	{
		Ray ballTrack = new Ray(enemyBody.position, enemyBody.velocity.normalized);
		RaycastHit boundaryHit;

		Physics.Raycast (ballTrack, out boundaryHit, 60f, mask);
		
		return boundaryHit;
	}

	Vector3 RandVectOnGround()
	{
		float rand = Random.Range (0f, Mathf.PI * 2);
		return new Vector3(Mathf.Cos(rand), 0f, Mathf.Sin(rand)); 
	}

	void MoveEnemy()
	{
		if (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.K) || Input.GetKey (KeyCode.L) || Input.GetKey (KeyCode.I)) 
		{
			float h = 0f;
			float v = 0f;

			if (Input.GetKey (KeyCode.L))
				h = 1f;
			if (Input.GetKey (KeyCode.J))
				h = -1f;
			if (Input.GetKey (KeyCode.I))
				v = 1f;
			if (Input.GetKey (KeyCode.K))
				v = -1f;
			enemyBody.velocity = new Vector3 (h, 0f, v);
		}
	}
	
}
