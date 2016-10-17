using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gameplay : MonoBehaviour 
{
	public Camera gameCamera;

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores

	public Slider setpointSlider;
	private Image sliderHandle;

	public SlaveController ball;
	public Controller[] verticalBats = new PlayerController[ 2 ];
	public Controller[] horizontalBats = new SlaveController[ 2 ];
	private PlayerController player = null;

	private int targetMask;

	private float error = 0.0f;

	private GameClient gameClient;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );

		player = verticalBats[ 0 ].GetComponent<PlayerController>();

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();
	}

	void Start()
	{
		gameClient = (GameClient) GameManager.GetConnection();
	}

	void FixedUpdate()
	{
		Vector3 impactPoint = ball.FindImpactPoint( targetMask );

		float setpoint = player.ControlPosition( impactPoint, out error );

		setpointSlider.value = setpoint;

		if( error >= 2 * PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.red;
		else if( error >= PlayerController.ERROR_THRESHOLD ) sliderHandle.color = Color.yellow;
		else sliderHandle.color = Color.green;
	}

	public void StartPlay()
	{
		int clientID = gameClient.ReceiveClientID();
		Debug.Log( "Client ID received: " + clientID.ToString() );
		if( clientID == 0 ) 
		{
			verticalBats[ 0 ].GetComponent<PlayerController>().enabled = true;
			verticalBats[ 1 ].GetComponent<PlayerController>().enabled = true;
			horizontalBats[ 0 ].GetComponent<SlaveController>().enabled = true;
			horizontalBats[ 1 ].GetComponent<SlaveController>().enabled = true;
			player = verticalBats[ 0 ].GetComponent<PlayerController>();
		} 
		else if( clientID == 1 ) 
		{
			horizontalBats[ 0 ].GetComponent<PlayerController>().enabled = true;
			horizontalBats[ 1 ].GetComponent<PlayerController>().enabled = true;
			verticalBats[ 0 ].GetComponent<SlaveController>().enabled = true;
			verticalBats[ 1 ].GetComponent<SlaveController>().enabled = true;
			player = horizontalBats[ 0 ].GetComponent<PlayerController>();
			gameCamera.transform.RotateAround( transform.position, Vector3.up, 90f );
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
