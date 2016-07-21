using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour {

	enum Status { paused, starting, playing, stopped, reset };

	static int timeToStart = 3;

	public float speed;	// Enemy speed
//	public float timeDelay = 0.5f;	// Delay for creating another "pickup"

	public GameObject pickUp;		// Pickup will appear on the next impact point
	public RaycastHit enemyTrack;
	public EnemyTarget currentPickUp;

	public Text defendedText, missedText, statusText; 	// UI Scores
	private int defended, missed;

//	private int pickUpMask;			// Mask for the playe where "pickup" will appear

	//public Training training;

	[HideInInspector] public Rigidbody enemyBody;		// Enemy rigid body

	public int onTagPickUp, onWallHit;
	public Collider show;

	private float startingTime;
	private int startingCounter;
	public int eventCounter;

	public Slider speedSlider;
	public Toggle visualHelp;

	public Button playButton, stopButton;

	[SerializeField] Status gameStatus;
	public HideShow resultPanel;

	void Awake(){
		
	}

	void Start () 
	{
		enemyBody = GetComponent<Rigidbody> ();
		gameStatus = Status.paused;
		enemyBody.velocity = 0.2f * Vector3.down + 0.1f * Vector3.one;
		startingTime = 0f;
		startingCounter = timeToStart;
		onTagPickUp = 0;
		onWallHit = 0;
		eventCounter = 0;
//		pickUpMask = LayerMask.GetMask ("PickUpWall");
		defended = missed = 0;
		currentPickUp = Instantiate(pickUp).gameObject.GetComponent<EnemyTarget>();
		currentPickUp.isActive = false;

		playButton.onClick.AddListener (StartPlay);
	}

	void Update()
	{
		defendedText.text = "Defended\n" + defended.ToString ("D");
		missedText.text = "Missed\n" + missed.ToString ("D");
		speed = speedSlider.value;
		DataManager.Instance.UpdateScore (defended, missed);

		switch (gameStatus)
		{
			case Status.starting:
			case Status.reset:
				if (startingTime < 1f)
				{
					SetTextStatus (startingCounter.ToString ("D"), Mathf.Lerp (1f, 1.4f, startingTime), 1f - startingTime * 1.2f);
					startingTime += Time.deltaTime;
				} else
				{
					startingTime = 0f;
					startingCounter--;
				}

				if (startingCounter == -1)
				{
					SetTextStatus ("GO!", Mathf.Lerp (1f, 1.4f, startingTime), 1f - startingTime * 1.2f);
				}

				if (startingCounter == -2)
				{
					gameStatus = Status.playing;
					SetTextStatus ("", 1f, 0f);
				}

				break;
			default:
				startingCounter = timeToStart;
				startingTime = 0f;
				break;
		}
	}

	void FixedUpdate()
	{
		if (Mathf.Abs (enemyBody.velocity.y) < 0.1f)
		{
			if (gameStatus == Status.playing)
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
		gameStatus = Status.starting;

		Text aux_text = playButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "PAUSE";
		aux_text.color = Color.red;
//		playButton.gameObject.GetComponentInChildren<Image> ().color = Color.red;
		playButton.onClick.RemoveListener (StartPlay);
		playButton.onClick.AddListener (StopPlay);
		stopButton.interactable = true;	
		resultPanel.Hide ();
		}
	public void StopPlay()
	{
		gameStatus = Status.paused;
		SetTextStatus("PAUSED", 1f, 1f);

		Text aux_text = playButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "Continue";
		aux_text.color = Color.black;
		playButton.gameObject.GetComponentInChildren<Image> ().color = new Color ((16f/255f), (204f/255f), 0f);;
		playButton.onClick.RemoveListener (StopPlay);
		playButton.onClick.AddListener (StartPlay);
	}

	public void Finish()
	{
		gameStatus = Status.stopped;
		SetTextStatus("", 1f, 1f);

		Text aux_text = playButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "START";
		aux_text.color = Color.black;
		playButton.gameObject.GetComponentInChildren<Image> ().color = new Color ((16f/255f), (204f/255f), 0f);;
		playButton.onClick.RemoveListener (StopPlay);
		playButton.onClick.AddListener (Reset);
		stopButton.interactable = false;
		resultPanel.Show ();
	}

	public void Reset()
	{
		enemyBody.position = new Vector3(0f, 23.5f, 0f); // Vector3.zero;
		enemyBody.velocity = Vector3.down;
		defended = 0;
		missed = 0;
		gameStatus = Status.reset;

		Text aux_text = playButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "PAUSE";
		aux_text.color = Color.red;
		//		playButton.gameObject.GetComponentInChildren<Image> ().color = Color.red;
		playButton.onClick.RemoveListener (Reset);
		playButton.onClick.AddListener (StopPlay);
		stopButton.interactable = true;	
		resultPanel.Hide ();
	}

	private void SetTextStatus(string text, float scale, float alpha)
	{
		statusText.text = text;
		statusText.gameObject.GetComponent<Transform> ().localScale = scale * Vector3.one;
		statusText.color = new Color (1f, 206f / 255f, 0f, alpha);

	}

	public int GameStatus()
	{
		return (int)gameStatus;
	}
}
