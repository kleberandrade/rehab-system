using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoxClashClient : GameClient 
{
	public Text localPlayerScoreText, remotePlayerScoreText;

	public Controller[] boxes = new Controller[ 2 ];

	private WavePlayerController player = null;

	//private int targetMask;

	private float error = 0.0f;

	private int clientID = -1;

	void Awake()
	{
		//targetMask = LayerMask.GetMask( "Target" );

		player = boxes[ 0 ].GetComponent<WavePlayerController>();

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

		//setpointSlider.value = setpoint;

		//if( error >= 2 * PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.red;
		//else if( error >= PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.yellow;
		//else sliderHandle.color = Color.green;
	}

	/*IEnumerator RegisterValues()
	{
		// Set log file names
		StreamWriter verticalLog = new StreamWriter( "./vertical" + clientID.ToString() + ".log", false );
		StreamWriter horizontalLog = new StreamWriter( "./horizontal" + clientID.ToString() + ".log", false );
		StreamWriter ballLog = new StreamWriter( "./ball" + clientID.ToString() + ".log", false );
		StreamWriter networkLog = new StreamWriter( "./network" + clientID.ToString() + ".log", false );

		while( Application.isPlaying )
		{
			ConnectionInfo currentConnectionInfo = connection.GetCurrentInfo();

			infoText.text =  string.Format( "Client: {0} Sent: {1} Received: {2} Lost Packets: {3} RTT: {4,3}ms", clientID,
				currentConnectionInfo.sentPackets, currentConnectionInfo.receivedPackets, currentConnectionInfo.lostPackets, currentConnectionInfo.rtt );

			if( ball.transform.position != lastBallPosition )
			{
				double gameTime = DateTime.Now.TimeOfDay.TotalSeconds;
				verticalLog.WriteLine( string.Format( "{0}\t{1}", gameTime, verticalBats[ 0 ].transform.position.z ) );
				horizontalLog.WriteLine( string.Format( "{0}\t{1}", gameTime, horizontalBats[ 0 ].transform.position.x ) );
				ballLog.WriteLine( string.Format( "{0}\t{1}\t{2}", gameTime, ball.transform.position.x, ball.transform.position.z ) );
				networkLog.WriteLine( string.Format( "{0}\t{1}", gameTime, currentConnectionInfo.rtt / 2.0f ) );
			}

			yield return new WaitForFixedUpdate();
		}

		verticalLog.Flush();
		horizontalLog.Flush();
		ballLog.Flush();
		networkLog.Flush();
	}*/

	IEnumerator HandleConnection()
	{
		while( clientID == -1 && Application.isPlaying )
		{
			clientID = connection.GetID();
			yield return new WaitForSeconds( 0.1f );
		}

		if( clientID == 0 ) 
		{
			boxes[ 0 ].GetComponent<WavePlayerController>().enabled = true;
			boxes[ 1 ].GetComponent<WaveSlaveController>().enabled = true;
			player = boxes[ 0 ].GetComponent<WavePlayerController>();
		} 
		else if( clientID == 1 ) 
		{
			boxes[ 0 ].GetComponent<WaveSlaveController>().enabled = true;
			boxes[ 1 ].GetComponent<WavePlayerController>().enabled = true;
			player = boxes[ 1 ].GetComponent<WavePlayerController>();
			gameCamera.transform.RotateAround( transform.position, Vector3.up, 180.0f );
		}

		//StartCoroutine( RegisterValues() );
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
		boxes[ 0 ].enabled = boxes[ 1 ].enabled = false;
	}
}