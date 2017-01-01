using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class DoublePongClient : GameClient 
{
	public SlaveController ball;
	private Vector3 lastBallPosition;

	public Text localPlayerScoreText, remotePlayerScoreText;

	public Controller[] verticalBats = new Controller[ 2 ];
	public Controller[] horizontalBats = new Controller[ 2 ];
	private PlayerController[] playerBats = new PlayerController[ 2 ];

	private int targetMask;

	private float error = 0.0f;

	private int clientID = -1;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );

		playerBats[ 0 ] = verticalBats[ 0 ].GetComponent<PlayerController>();
		playerBats[ 1 ] = verticalBats[ 1 ].GetComponent<PlayerController>();

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();

		lastBallPosition = ball.transform.position;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

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
			ConnectionInfo currentConnectionInfo = connection.GetConnectionInfo();

			int networkDelay = connection.GetNetworkDelay();

			infoText.text =  string.Format( "Client: {0} Server Uptime: {1:F1}s Last Delay: {2}ms\nSend: {3,2}KB/s Receive: {4,2}KB/s RTT: {5,3}ms Lost Packets: {6}", clientID, gameTime,
				                            networkDelay, currentConnectionInfo.sendRate, currentConnectionInfo.receiveRate, currentConnectionInfo.rtt, currentConnectionInfo.lostPackets );

			if( ball.transform.position != lastBallPosition )
			{
				float gameTime = DateTime.Now.TimeOfDay.TotalSeconds;
				verticalLog.WriteLine( string.Format( "{0}\t{1}", gameTime, verticalBats[ 0 ].transform.position.z ) );
				horizontalLog.WriteLine( string.Format( "{0}\t{1}", gameTime, horizontalBats[ 0 ].transform.position.x ) );
				ballLog.WriteLine( string.Format( "{0}\t{1}\t{2}", gameTime, ball.transform.position.x, ball.transform.position.z ) );
				networkLog.WriteLine( string.Format( "{0}\t{1}\t{2}", gameTime, currentConnectionInfo.rtt, networkDelay ) );
			}

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
			clientID = connection.GetClientID();
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
			connection.Connect();
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
