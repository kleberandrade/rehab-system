using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Multiplayer : MonoBehaviour 
{
	public BallController ball;
	public BatController player;
	public BatController bat;

	private int targetMask;

	private float error = 0.0f;

	void Start()
	{
		targetMask = LayerMask.GetMask( "Target" );
	}

	void FixedUpdate()
	{
		Vector3 impactPoint = ball.FindImpactPoint( targetMask );
	}
}
