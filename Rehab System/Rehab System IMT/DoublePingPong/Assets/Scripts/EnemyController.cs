using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed;	// Enemy speed
//	public float timeDelay = 0.5f;	// Delay for creating another "pickup"

	public GameObject pickUp;		// Pickup will appear on the next impact point
	public RaycastHit enemyTrack;

	private int pickUpMask;			// Mask for the playe where "pickup" will appear

	[HideInInspector] public Rigidbody enemyBody;		// Enemy rigid body

	public int onTagPickUp, onWallHit;
	public bool missWall;
	public Collider show;

	void Awake(){
		enemyBody = GetComponent<Rigidbody> ();
	}

	void Start () 
	{
//		enemyBody.velocity = RandVectOnGround()*speed;	// Inicialize with a random velocity
		enemyBody.velocity = 0.2f * Vector3.down + 0.1f * Vector3.one;
		onTagPickUp = 0;
		onWallHit = 0;
		missWall = false;
		pickUpMask = LayerMask.GetMask ("PickUp");
	}

	void FixedUpdate()
	{
		if (Mathf.Abs(enemyBody.velocity.y) < Mathf.Epsilon)
		{
			if (Mathf.Abs(enemyBody.velocity.magnitude) < Mathf.Epsilon)
			{
				enemyBody.velocity = RandVectOnGround()*speed;
				if ((enemyTrack = FindImpact(pickUpMask)).point != Vector3.zero)
					Instantiate(pickUp, enemyTrack.point, Quaternion.identity);	// Instantiate a new "pickup" 
			}
			else
			{
				enemyBody.velocity = enemyBody.velocity.normalized * speed;
			}
		}

		// Counting delay for new "pickup" 
//		pickUpTimeCount += Time.deltaTime;	

	//	GameObject aux = GameObject.FindGameObjectsWithTag("PickUp")
		//if (GameObject.FindGameObjectWithTag("PickUp") == null)
	//	if (pickUpTimeCount > 0f)
//		{
//			enemyTrack = FindImpact(pickUpMask);
//			Instantiate(pickUp, enemyTrack.point, Quaternion.identity);	// Instantiate a new "pickup" 
//			pickUpTimeCount = -3f;
//
//		}

		// Alternative enemy control for testing
		MoveEnemy();
	}

	void HitWall(string wall)
	{
//		if (Mathf.Abs(multiHitCheck - Time.time) <= Mathf.Epsilon)
//		{
//			enemyBody.velocity = new Vector3(-enemyBody.position.x, 0f, -enemyBody.position.z);
//			return;
//		}
		if (onWallHit > 1)
			enemyBody.velocity = new Vector3 (-enemyBody.position.x, 0f, -enemyBody.position.z).normalized * speed;
		else 
		{
			switch (wall) {
			case "Vertical":
				enemyBody.velocity = new Vector3 (-enemyBody.velocity.x, 0f, Random.Range (-speed, speed));
				break;
			case "Horizontal": 
				enemyBody.velocity = new Vector3 (Random.Range (-speed, speed), 0f, -enemyBody.velocity.z);
				break;
			case "Tower":
				enemyBody.velocity = new Vector3 (-enemyBody.velocity.x, 0f, -enemyBody.velocity.z);
				break;
			}
		}
	//	multiHitCheck = Time.time;
		enemyBody.velocity = enemyBody.velocity.normalized*speed;
//		pickUpTimeCount = 0f;
	}

	void OnTriggerEnter(Collider other)
	{
		show = other;
		switch (other.gameObject.tag) 
		{
		case "Vertical":
		case "Horizontal":
		case "Tower":
			onWallHit ++;
			HitWall(other.gameObject.tag); 
			break;
		case "PickUp":
			Destroy(other.gameObject);
			break;
		case "Sky":
			enemyBody.position = new Vector3(0f, 23.5f, 0f); // Vector3.zero;
			enemyBody.velocity = Vector3.zero;
			break;
		case "PickUpWall":
			onTagPickUp ++;
			break;
		case "Boundary":
			missWall = true;
			break;
		}
	}

	void OnTriggerExit(Collider other)
	{
		switch (other.gameObject.tag) 
		{
		case "Vertical":
		case "Horizontal":
		case "Tower":
			onWallHit --;
			break;
		case "PickUpWall":
			onTagPickUp --;
			if (onTagPickUp == 0)
			{
				enemyTrack = FindImpact(pickUpMask);
				Instantiate(pickUp, enemyTrack.point, Quaternion.identity);	// Instantiate a new "pickup" 
			}
			break;
		case "Boundary":
			missWall = false;
			break;
		}
	}

	public RaycastHit FindImpact(int mask)
	{
		RaycastHit boundaryHit;

		Physics.Raycast(enemyBody.position, enemyBody.velocity, out boundaryHit, 60f, mask);
		
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
