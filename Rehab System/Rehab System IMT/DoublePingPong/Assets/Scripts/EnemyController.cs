using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed;	// Enemy speed
//	public float timeDelay = 0.5f;	// Delay for creating another "pickup"

	public GameObject pickUp;		// Pickup will appear on the next impact point
	public RaycastHit enemyTrack;
	public EnemyTarget currentPickUp;

	public Text defendedText, missedText; 	// UI Scores
	private int defended, missed;

	private int pickUpMask;			// Mask for the playe where "pickup" will appear

	//public Training training;

	[HideInInspector] public Rigidbody enemyBody;		// Enemy rigid body

	public int onTagPickUp, onWallHit;
	public Collider show;

	public bool playing;
	public int eventCounter;

	public Slider speedSlider;
	public Toggle visualHelp;

	void Awake(){
		enemyBody = GetComponent<Rigidbody> ();
	}

	void Start () 
	{
		playing = false;
//		enemyBody.velocity = RandVectOnGround()*speed;	// Inicialize with a random velocity
		enemyBody.velocity = 0.2f * Vector3.down + 0.1f * Vector3.one;
		onTagPickUp = 0;
		onWallHit = 0;
		eventCounter = 0;
		pickUpMask = LayerMask.GetMask ("PickUpWall");
		defended = missed = 0;
		currentPickUp = Instantiate(pickUp).gameObject.GetComponent<EnemyTarget>();
		currentPickUp.isActive = false;
	}

	void Update()
	{
		defendedText.text = "Defended\n" + defended.ToString ("D");
		missedText.text = "Missed\n" + missed.ToString ("D");
		speed = speedSlider.value;
	}

	void FixedUpdate()
	{
		if (Mathf.Abs (enemyBody.velocity.y) < 0.1f)
		{
			if (playing)
			{
				if (Mathf.Abs (enemyBody.velocity.magnitude) < 0.1f)
				{
//					if (training.plan.Count == 0)
					enemyBody.velocity = RandVectOnGround () * speed;
					UpdatePickUp ();
//					if ((enemyTrack = FindImpact(pickUpMask)).point != Vector3.zero)
//						Instantiate(pickUp, enemyTrack.point, Quaternion.identity);	// Instantiate a new "pickup" 
				} else
				{
					enemyBody.velocity = enemyBody.velocity.normalized * speed;
				}

			} else
			{
				//		enemyBody.position = new Vector3(enemyBody.position.x,
				//		                                 -0.5f,
				//		                                 enemyBody.position.z);
				enemyBody.angularVelocity = Vector3.zero;
				enemyBody.velocity = Vector3.zero;
				UpdatePickUp (false);
			}
		} else
			UpdatePickUp (false);

		visualHelp.onValueChanged.AddListener (delegate
			{
				UpdatePickUp ();
			});

		// Alternative enemy control for testing
		MoveEnemy();
	}

	void UpdatePickUp()
	{
		currentPickUp.isActive = visualHelp.isOn;
	}
	void UpdatePickUp(bool value)
	{
		currentPickUp.isActive = value;
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
					defended++;
					break;
				case "Horizontal": 
					enemyBody.velocity = new Vector3 (Random.Range (-speed, speed), 0f, -enemyBody.velocity.z);
					defended++;
					break;
				case "Tower":
					enemyBody.velocity = new Vector3 (-enemyBody.velocity.x, 0f, -enemyBody.velocity.z);
					missed++;
					break;
			}
			eventCounter++;
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
				UpdatePickUp(false);
				break;
			case "Sky":
					enemyBody.position = new Vector3(0f, 23.5f, 0f); // Vector3.zero;
					enemyBody.velocity = Vector3.down;
					missed++;
					break;
			case "PickUpBoundary":
				onTagPickUp++;
				break;
//			case "Boundary":
//				break;
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
			case "PickUpBoundary":
				onTagPickUp--;
				if (onTagPickUp == 0)
					UpdatePickUp ();
				break;
//			case "Boundary":
//				UpdatePickUp (false);
//				break;
		}
	}

	public RaycastHit FindImpact(int mask)
	{
		RaycastHit boundaryHit;

		Physics.Raycast (enemyBody.position, enemyBody.velocity, out boundaryHit, 60f, mask);
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

	public void StartPlay()
	{
		playing = true;
	}
	public void StopPlay()
	{
		playing = false;
	}

}
