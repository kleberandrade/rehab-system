using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;


public class ToAnkleRobot : MonoBehaviour {
	private const int VERTICAL = 0;		// or RIGHT? 	DP - Dorsiflexion/Plantarflexion
	private const int HORIZONTAL = 1;	// or LEFT?		IE - Inversion/Eversion
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

	public bool activeConnection, activeHelper, followBall, elipseSpace, activeDisturber;

	public Text playerScoreText, machineScoreText, lazyScoreText, gameTimeText; 	// UI Scores
	public Text messageText;
	private string connecting, playing;

	[SerializeField] private float playerScore, machineScore, lazyScore, gameTime;				// Value Scores
	public float lazySpeed, lazyForce;
	
	// Envelope do movimento
	public Vector2 max, min;		// Input for elipse
	public Vector2 bases, origin;	// Elipse's parameters
	public float elipseScale;		// Scale for fitting the moves
	public float squareScale = 1f;

	// Communication with another scripts
	public PlayerController player;
	public EnemyController enemy;
	public Connection connection;
	private Logger logger;

	// Communication
	public Vector2 input, enemyPos;

	// Control
	private int targetMask;
	public float helperLimit;
	public float helperFade;

//	private float fm, fa, dfm, dfa, dt;
	private int eventCounter;

	public Button connectButton, startButton, stopButton;
	public InputField totalTime;

	[Space]

	public Vector2 wallPos;
	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float K, D;				// Stiffness and Damping

	void Awake () 
	{
		PlayerPrefs.DeleteAll ();

		targetMask = LayerMask.GetMask ("Target");
		
		playerScoreText.text = "Moving\n0";
		machineScoreText.text = "Machine\n0";
		lazyScoreText.text = "Lazy\n0";
		gameTimeText.text = "Total\n0";
		playerScore = 0f;
		machineScore = 0f;
		lazyScore = 0f;
		gameTime = 0f;
		connecting = "No connection.";
		playing = "Game Stopped.";
	}

	void Start ()
	{
		activeConnection = false;
//		connection = GetComponent<Connection>();
//		fm = fa = dt = dfm = dfa = 0f;
		eventCounter = enemy.eventCounter;
		startButton.interactable = false;
		stopButton.interactable = false;
		connectButton.onClick.AddListener (Connect);
	}

	void Update()
	{
		// Update scores
//		lazyScore = Time.time - (playerScore + machineScore);
		playerScoreText.text = "Moving\n" + TimeFormat(playerScore, "00.0", 1);
		machineScoreText.text = "Machine\n" + TimeFormat(machineScore, "00.0", 1);
		lazyScoreText.text = "Lazy\n" + TimeFormat(lazyScore, "00.0", 1);
		gameTimeText.text = "Total\n" + TimeFormat(gameTime, "00", 0);
		messageText.text = connecting + "\n" + playing;

		if (totalTime.text != "")
			if ((gameTime > 60 * int.Parse (totalTime.text)) && (enemy.GameStatus() == 2))
			{
				enemy.Finish ();
			}

		DataManager.Instance.UpdateROM (playerScore, lazyScore, max.y, min.y, max.x, min.x);

		if (connection != null)
		{
			activeConnection = connection.IsConnected ();
		} else
			activeConnection = false;

		startButton.interactable = activeConnection;
	}

	void FixedUpdate () 
	{

		switch (enemy.GameStatus ())
		{
			case 0:
				playing = "Game Paused.";
				break;
			case 2:
				playing = "Playing!";
				gameTime += Time.deltaTime;
				break;
			case 3:
				playing = "Game Stopped.";
				if (totalTime.text == "")
					totalTime.placeholder.GetComponent<Text> ().text = "Play Time in minutes";
				totalTime.interactable = true;
				if (logger != null) Destroy (logger);
				break;
			default:
				logger = gameObject.AddComponent<Logger> ();
				totalTime.interactable = false;
				if (totalTime.text == "")
					totalTime.placeholder.GetComponent<Text> ().text = "Infinity time";
				playing = "Starting.";
				break;
		}

		// Follow the ball
		enemyPos = new Vector2 (enemy.enemyBody.position.x, enemy.enemyBody.position.z);

		if (elipseSpace)
			enemyPos = SquareToElipse (enemyPos);
		else
			enemyPos = enemyPos / squareScale / player.boundaryDist/3f;
		
		if (activeConnection)
		{
			connecting = "Connected!";

			input = new Vector2
				(
				connection.ReadStatus(HORIZONTAL, Connection.POSITION),
				connection.ReadStatus(VERTICAL, Connection.POSITION)
				);

			// Move player
			if (elipseSpace)
				wallPos = ElipseToSquare (input);
			else
				wallPos = input * squareScale;
			
			player.SetWalls(wallPos);

			// Player helper
			if (activeHelper)
				PlayerHelper ();

			if (activeDisturber)
				PlayerDisturber ();

			if (enemy.GameStatus () == 2)
			{
				if ((new Vector2 (connection.ReadStatus (HORIZONTAL, Connection.FORCE),
					    connection.ReadStatus (HORIZONTAL, Connection.FORCE)).magnitude > lazyForce) && activeHelper)
					machineScore += Time.deltaTime;
				else if (new Vector2 (connection.ReadStatus (HORIZONTAL, Connection.VELOCITY),
					         connection.ReadStatus (HORIZONTAL, Connection.VELOCITY)).magnitude > lazySpeed)
					playerScore += Time.deltaTime;
				else
					lazyScore += Time.deltaTime;
				logger.Register ();
			}

				if (followBall)
					centerSpring = enemyPos;
			
			// Set variables to send to robot
			connection.SetStatus ((short)enemy.GameStatus ());

			connection.SetStatus (VERTICAL, centerSpring.y, Connection.CENTERSPRING);
			connection.SetStatus (HORIZONTAL, centerSpring.x, Connection.CENTERSPRING);
			connection.SetStatus (VERTICAL, freeSpace.y, Connection.FREESPACE);
			connection.SetStatus (HORIZONTAL, freeSpace.x, Connection.FREESPACE);

			connection.SetStatus (VERTICAL, K, Connection.STIFF);
			connection.SetStatus (HORIZONTAL, K, Connection.STIFF);
			connection.SetStatus (VERTICAL, D, Connection.DAMP);
			connection.SetStatus (HORIZONTAL, D, Connection.DAMP);



		} else 
		{
			connecting = "Disconnected!";
			player.MoveWalls(player.ReadInput());
			switch (enemy.GameStatus ())
			{
				case 2:
					if (player.ReadInput ().magnitude > lazySpeed)
						playerScore += Time.deltaTime;
					else
						lazyScore += Time.deltaTime;
					break;
				case 4:
					playerScore = 0f;
					lazyScore = 0f;
					gameTime = 0f;
					break;
			}

			wallPos = new Vector2
				(
				player.horizontalWalls [0].position.x/player.boundary/3f,
				player.verticalWalls [0].position.z/player.boundary/3f
				);
			input = wallPos / player.boundary * 5f;
		}
		Calibration (input);
	}

	string TimeFormat(float time, string format, int round)
	{
		return Mathf.FloorToInt(time / 60).ToString ("D2") + ":" + (Round(time, round) % 60).ToString (format);
	}

	float Round (float realNum, int decPlaces)
	{
		int aux = Mathf.FloorToInt(realNum * Mathf.Pow (10f, (float)decPlaces));
		return aux / Mathf.Pow (10f, decPlaces);
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
		Vector2 impact, safeArea, track, distance;
		float impactDist;

		impactDist = enemy.FindImpact (targetMask).distance + helperLimit;

		impact = new Vector2 (
			enemy.FindImpact(targetMask).point.x, 
			enemy.FindImpact(targetMask).point.z);

		safeArea = new Vector2 (
			Mathf.Clamp(player.boundary - Mathf.Abs (impact.y), 0f, player.boundary),
			Mathf.Clamp(player.boundary - Mathf.Abs (impact.x), 0f, player.boundary));

		track = new Vector2 (impactDist, impactDist);
		
		distance = (track + safeArea) / enemy.speed * player.speed;

		if (elipseSpace)
		{
			if (helperFade > 1f)
			{
				if ((centerSpring - SquareToElipse (impact)).magnitude < 0.05f)
				{
					centerSpring = SquareToElipse (impact);
					freeSpace = SquareToElipse (distance);
				} else
				{
					helperFade = 0f;
				}
			} else
			{
				centerSpring = Vector2.Lerp (centerSpring, SquareToElipse (impact), helperFade);
				freeSpace = Vector2.Lerp (freeSpace, SquareToElipse (distance), helperFade);
				helperFade += Time.deltaTime;
			}
		} else
		{
			if (helperFade > 1f)
			{
				if ((centerSpring - (impact / squareScale / player.boundaryDist)).magnitude < 0.05f)
				{
					centerSpring = impact / squareScale / player.boundaryDist;
					freeSpace = distance / squareScale / player.boundaryDist;
				} else
				{
					helperFade = 0f;
				}
			} else
			{
				centerSpring = Vector2.Lerp (centerSpring, impact / squareScale / player.boundaryDist, helperFade);
				freeSpace = Vector2.Lerp (freeSpace, distance / squareScale / player.boundaryDist, helperFade);
				helperFade += Time.deltaTime;
			}
		}
	}

	void PlayerDisturber()
	{
		if (helperFade >= 0.2f)
		{
			if (eventCounter != enemy.eventCounter)
			{
				helperFade = 0f;
				eventCounter = enemy.eventCounter;
				Debug.Log ("Event: " + eventCounter + " " + input);
			} 
		}
		else
		{
			centerSpring = Vector2.Lerp (centerSpring, input, helperFade * 5f);
//			if (elipseSpace)
				freeSpace = bases / 3;
//			else
//				freeSpace = new Vector2(
			
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

	public void Connect()
	{
		connection = gameObject.AddComponent<Connection> ();
/*		logger = gameObject.AddComponent<Logger> ();
		//connection = GetComponent<Connection> ();
		//logger = GetComponent<Logger> ();
		logger.connection = connection;
		logger.robot = this;
*/
		Text aux_text = connectButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "Disconnect";
		aux_text.color = new Color (1f, (216f/255f), 0f);
		connectButton.gameObject.GetComponentInChildren<Image> ().color = Color.red;
		connectButton.onClick.RemoveListener (Connect);
		connectButton.onClick.AddListener (Disconnect);
		connectButton.onClick.AddListener (enemy.Finish);
		connecting = "Trying to connect...";
	}

	public void Disconnect()
	{
		//connection.CloseConnection();
		if (connection != null) Destroy (connection);
//		connection = null;

		Text aux_text = connectButton.gameObject.GetComponentInChildren<Text> ();
		aux_text.text = "Connect";
		aux_text.color = Color.black;
		connectButton.gameObject.GetComponentInChildren<Image> ().color = new Color(0f, (192f/255f), 1f);
		connectButton.onClick.RemoveListener (Disconnect);
		connectButton.onClick.AddListener (Connect);
		startButton.interactable = false;
		stopButton.interactable = false;
		connecting = "Connection aborted.";
	}

}
