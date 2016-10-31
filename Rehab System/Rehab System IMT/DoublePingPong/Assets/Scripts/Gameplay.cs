using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class Gameplay : MonoBehaviour 
{
	public Camera gameCamera;

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores

	public Slider setpointSlider;
	private Image sliderHandle;

	public SlaveController ball;
	private Vector3 lastBallPosition;

	public Controller[] verticalBats = new Controller[ 2 ];
	public Controller[] horizontalBats = new Controller[ 2 ];
	private PlayerController[] playerBats = new PlayerController[ 2 ];

	private int targetMask;

	private float error = 0.0f;

	private GameClient gameClient;
	private int clientID = -1;

	private DateTime gameTime;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );

		playerBats[ 0 ] = verticalBats[ 0 ].GetComponent<PlayerController>();
		playerBats[ 1 ] = verticalBats[ 1 ].GetComponent<PlayerController>();

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();

		lastBallPosition = ball.transform.position;
	}

	void Start()
	{
		gameClient = (GameClient) GameManager.GetConnection();

		gameTime = NTPClient.GetNetworkTime().ToUniversalTime();
	}

	void FixedUpdate()
	{
		Vector3 impactPoint = ball.FindImpactPoint( targetMask );

		playerBats[ 1 ].ControlPosition( impactPoint, out error );
		float setpoint = playerBats[ 0 ].ControlPosition( impactPoint, out error );

		setpointSlider.value = setpoint;

		if( error >= 2 * PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.red;
		else if( error >= PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.yellow;
		else sliderHandle.color = Color.green;
	}

	IEnumerator RegisterValues()
	{
		// Set log file names
		StreamWriter verticalLog = new StreamWriter( "./vertical" + clientID.ToString() + ".log", false );
		StreamWriter horizontalLog = new StreamWriter( "./horizontal" + clientID.ToString() + ".log", false );
		StreamWriter ballLog = new StreamWriter( "./ball" + clientID.ToString() + ".log", false );
		StreamWriter networkLog = new StreamWriter( "./network" + clientID.ToString() + ".log", false );

		while( Application.isPlaying )
		{
			ConnectionInfo currentConnectionInfo = gameClient.GetConnectionInfo();

			lazyScoreText.text =  string.Format( "Client: {0} Game Time: {1}\nSend: {2,2}KB/s Receive: {3,2}KB/s RTT: {4,3}ms Lost Packets: {5}", clientID, gameTime.TimeOfDay.ToString(),
				                                 currentConnectionInfo.sendRate, currentConnectionInfo.receiveRate, currentConnectionInfo.rtt, currentConnectionInfo.lostPackets );

			if( ball.transform.position != lastBallPosition )
			{
				double sampleTime = gameTime.TimeOfDay.TotalSeconds;

				verticalLog.WriteLine( string.Format( "{0}\t{1}", sampleTime, verticalBats[ 0 ].transform.position.z ) );
				horizontalLog.WriteLine( string.Format( "{0}\t{1}", sampleTime, horizontalBats[ 0 ].transform.position.x ) );
				ballLog.WriteLine( string.Format( "{0}\t{1}\t{2}", sampleTime, ball.transform.position.x, ball.transform.position.z ) );
				networkLog.WriteLine( string.Format( "{0}\t{1}", sampleTime, currentConnectionInfo.rtt ) );
			}

			gameTime = gameTime.AddSeconds( Time.fixedDeltaTime );

			yield return new WaitForFixedUpdate();
		}

		verticalLog.Flush();
		horizontalLog.Flush();
		ballLog.Flush();
		networkLog.Flush();
	}

	IEnumerator HandleConnection()
	{
		while( clientID == -1 && Application.isPlaying )
		{
			clientID = gameClient.GetClientID();
			yield return new WaitForSeconds( 0.1f );
		}

		if( clientID == 0 ) 
		{
			verticalBats[ 0 ].GetComponent<PlayerController>().enabled = true;
			verticalBats[ 1 ].GetComponent<PlayerController>().enabled = true;
			horizontalBats[ 0 ].GetComponent<SlaveController>().enabled = true;
			horizontalBats[ 1 ].GetComponent<SlaveController>().enabled = true;
			playerBats[ 0 ] = verticalBats[ 0 ].GetComponent<PlayerController>();
			playerBats[ 1 ] = verticalBats[ 1 ].GetComponent<PlayerController>();
		} 
		else if( clientID == 1 ) 
		{
			horizontalBats[ 0 ].GetComponent<PlayerController>().enabled = true;
			horizontalBats[ 1 ].GetComponent<PlayerController>().enabled = true;
			verticalBats[ 0 ].GetComponent<SlaveController>().enabled = true;
			verticalBats[ 1 ].GetComponent<SlaveController>().enabled = true;
			playerBats[ 0 ] = horizontalBats[ 0 ].GetComponent<PlayerController>();
			playerBats[ 1 ] = horizontalBats[ 1 ].GetComponent<PlayerController>();
			gameCamera.transform.RotateAround( transform.position, Vector3.up, 90f );
		}

		ball.enabled = true;
		StartCoroutine( RegisterValues() );
	}

	public void StartPlay()
	{
		if( clientID == -1 )
		{
			gameClient.Connect();
			StartCoroutine( HandleConnection() );
		}
	}

	public void StopPlay()
	{
		ball.enabled = false;
		verticalBats[ 0 ].enabled = verticalBats[ 1 ].enabled = false;
		horizontalBats[ 0 ].enabled = horizontalBats[ 1 ].enabled = false;
	}
}
