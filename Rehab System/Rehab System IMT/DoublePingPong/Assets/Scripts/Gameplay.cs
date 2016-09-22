using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gameplay : MonoBehaviour 
{
	public Slider setpointSlider;
	private Image sliderHandle;

	public BallController ball;
	public PlayerController player;
	public SlaveController bat;

	private int targetMask;

	private float error = 0.0f;

	void Start()
	{
		targetMask = LayerMask.GetMask( "Target" );

		sliderHandle = setpointSlider.handleRect.GetComponent<Image>();
	}

	void FixedUpdate()
	{
		Vector3 impactPoint = ball.FindImpactPoint( targetMask );

		float setpoint = player.ControlPosition( impactPoint, out error );

		setpointSlider.value = setpoint;

		if( error >= 0.7f ) sliderHandle.color = Color.red;
		else if( error >= 0.3f ) sliderHandle.color = Color.yellow;
		else sliderHandle.color = Color.green;
	}
}
