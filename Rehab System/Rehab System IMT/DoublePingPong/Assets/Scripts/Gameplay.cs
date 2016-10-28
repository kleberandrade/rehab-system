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

	public Controller[] verticalBats = new PlayerController[ 2 ];
	public Controller[] horizontalBats = new SlaveController[ 2 ];
	private PlayerController[] playerBats = new PlayerController[ 2 ];

	private int targetMask;

	private float error = 0.0f;

	private GameClient gameClient;
	private int clientID = -1;

	protected string playerLogFile, slaveLogFile, ballLogFile, networkLogFile;

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

		// Set log file names
		playerLogFile = "./player" + GetInstanceID().ToString() + ".log";
		if( File.Exists( playerLogFile ) ) File.Delete( playerLogFile );
		slaveLogFile = "./slave" + GetInstanceID().ToString() + ".log";
		if( File.Exists( slaveLogFile ) ) File.Delete( slaveLogFile );
		ballLogFile = "./ball" + GetInstanceID().ToString() + ".log";
		if( File.Exists( ballLogFile ) ) File.Delete( ballLogFile );
		networkLogFile = "./network" + GetInstanceID().ToString() + ".log";
		if( File.Exists( networkLogFile ) ) File.Delete( networkLogFile );
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

		ConnectionInfo currentConnectionInfo = gameClient.GetConnectionInfo();

		lazyScoreText.text =  string.Format( "Socket: {0} Connection: {1} Channel: {2}\nSend: {3,2}KB/s Receive: {4,2}KB/s RTT: {5,3}ms Lost Packets: {6}", 
			                                 currentConnectionInfo.socketID, currentConnectionInfo.connectionID, currentConnectionInfo.channel,
			                                 currentConnectionInfo.sendRate, currentConnectionInfo.receiveRate, currentConnectionInfo.rtt, currentConnectionInfo.lostPackets );

		if( ball.transform.position != lastBallPosition )
		{
			File.AppendAllText( playerLogFile, Time.realtimeSinceStartup.ToString() + "\t" + verticalBats[ 0 ].transform.position.x.ToString() + "\t" + verticalBats[ 0 ].transform.position.z.ToString() + Environment.NewLine );
			File.AppendAllText( slaveLogFile, Time.realtimeSinceStartup.ToString() + "\t" + horizontalBats[ 0 ].transform.position.x.ToString() + "\t" + horizontalBats[ 0 ].transform.position.z.ToString() + Environment.NewLine );
			File.AppendAllText( ballLogFile, Time.realtimeSinceStartup.ToString() + "\t" + ball.transform.position.x.ToString() + "\t" + ball.transform.position.z.ToString() + Environment.NewLine );
			File.AppendAllText( networkLogFile, Time.realtimeSinceStartup.ToString() + "\t" + currentConnectionInfo.rtt.ToString() + Environment.NewLine );
		}
	}

	public void StartPlay()
	{
		if( clientID == -1 )
		{
			clientID = gameClient.ReceiveClientID();
			Debug.Log( "Client ID received: " + clientID.ToString() );
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
		}

		if( clientID != -1 ) ball.enabled = true;
	}

	public void StopPlay()
	{
		ball.enabled = false;
		verticalBats[ 0 ].enabled = verticalBats[ 1 ].enabled = false;
		horizontalBats[ 0 ].enabled = horizontalBats[ 1 ].enabled = false;
	}
}
