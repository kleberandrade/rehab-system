using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gameplay : MonoBehaviour 
{
	public Camera camera;

	public Slider setpointSlider;
	private Image sliderHandle;

	public TargetController ball;
	public PlayerController[] players = new PlayerController[ 2 ];
	public SlaveController[] opponents = new SlaveController[ 2 ];

	private int targetMask;

	private float error = 0.0f;

	private GameClient gameClient;

	void Awake()
	{
		targetMask = LayerMask.GetMask( "Target" );

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();

		gameClient = (GameClient) GameManager.GetGameConnection();
	}

	void FixedUpdate()
	{
		Vector3 impactPoint = ball.FindImpactPoint( targetMask );

		float setpoint = players[ 0 ].ControlPosition( impactPoint, out error );

		setpointSlider.value = setpoint;

		if( error >= 0.7f ) sliderHandle.color = Color.red;
		else if( error >= 0.3f ) sliderHandle.color = Color.yellow;
		else sliderHandle.color = Color.green;
	}

	public void StartPlay()
	{
		int clientID = gameClient.ReceiveClientID();
		if( clientID == 0 ) 
		{
			players[ 0 ].elementID = players[ 1 ].elementID = 0;
			opponents[ 0 ].elementID = opponents[ 1 ].elementID = 1;
		} 
		else if( clientID == 1 ) 
		{
			players[ 0 ].elementID = players[ 1 ].elementID = 1;
			opponents[ 0 ].elementID = opponents[ 1 ].elementID = 0;
			camera.transform.RotateAround( transform.position, Vector3.up, 90f );
		}

		if( clientID != -1 ) 
		{
			ball.enabled = true;
			players[ 0 ].enabled = players[ 1 ].enabled = true;
			opponents[ 0 ].enabled = opponents[ 1 ].enabled = true;
		}
	}

	public void StopPlay()
	{
		ball.enabled = false;
		players[ 0 ].enabled = players[ 1 ].enabled = false;
		opponents[ 0 ].enabled = opponents[ 1 ].enabled = false;
	}
}
