using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class PlayerController : Controller 
{
	const int POSITION = 0, VELOCITY = 1;
	public const float ERROR_THRESHOLD = 0.35f;

	public MeshRenderer moveBoxRenderer;

	public MeshFilter moveBoxPlane;
	private Vector3 initialMoveBoxPosition = Vector3.zero;
	private Vector3 initialMoveBoxScale = Vector3.zero;

	private float currentTargetSetpoint = 0.0f;
	private float movementInitialPosition = 0.0f;
	private float movementTime = 0.0f;
	const float MAX_MOVEMENT_TIME = 2.0f;

	private InputAxis controlAxis = null;
	private bool helperEnabled = false;

	void Start()
	{
		initialMoveBoxScale = transform.lossyScale;
	}

	void FixedUpdate()
	{
		if( controlAxis != null )
		{
			float input = controlAxis.NormalizedPosition;
			Debug.Log( "Input position: " + ( transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x ) ).ToString () );
			body.MovePosition( transform.right * ( Mathf.Clamp( input, -1.0f, 1.0f ) * rangeLimits.x ) + initialPosition );

			// Send locally controlled object position over network
			GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, POSITION, body.position.x );
			GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, POSITION, body.position.z );
			GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, VELOCITY, body.velocity.x );
			GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, VELOCITY, body.velocity.z );
		}
	}       

	public float ControlPosition( Vector3 target, out float error )
	{
		float batLength = Vector3.Dot( size, transform.right );

		float currentPosition = Vector3.Dot( body.position - initialPosition, transform.right );
		float maxSetpoint = Vector3.Dot( rangeLimits, transform.right );

		float targetSetpoint = Vector3.Dot( target - initialPosition, transform.right );
		targetSetpoint = Mathf.Clamp( targetSetpoint, -maxSetpoint, maxSetpoint );

		if( Mathf.Abs( currentTargetSetpoint - targetSetpoint ) > batLength / 2.0f ) 
		{
			movementTime = 0.0f;
			movementInitialPosition = currentPosition;
		}

		currentTargetSetpoint = 0.95f * currentTargetSetpoint + 0.05f * targetSetpoint;

		float currentSetpoint = Mathf.Lerp( movementInitialPosition, currentTargetSetpoint, movementTime / MAX_MOVEMENT_TIME );
		movementTime += Time.deltaTime;

		error = Mathf.Abs( ( currentSetpoint - currentPosition ) / maxSetpoint );

		moveBoxRenderer.transform.position = initialMoveBoxPosition + transform.right * ( currentTargetSetpoint + currentSetpoint ) / 2.0f;
		float moveBoxClearance = Mathf.Abs( ( targetSetpoint - currentSetpoint ) / batLength );
		//moveBoxRenderer.transform.localScale = Vector3.Scale( initialMoveBoxScale, new Vector3( 1.0f + moveBoxClearance, 1.0f, 1.0f ) );
		moveBoxRenderer.transform.localScale = new Vector3( 0.55f * ( 1.0f + moveBoxClearance ), 1.0f, 0.08f );
		moveBoxRenderer.material.color = new Color( 0.0f, 1.0f, 0.0f, 0.5f );

		if( controlAxis != null && helperEnabled && error > ERROR_THRESHOLD )
		{
			//Debug.Log( "Outside move box: (error: " + error.ToString() + ")" );
			controlAxis.NormalizedPosition = currentSetpoint / maxSetpoint;
			controlAxis.NormalizedVelocity = ( targetSetpoint - movementInitialPosition ) / ( MAX_MOVEMENT_TIME * maxSetpoint );
			controlAxis.Stiffness = 30.0f;

			moveBoxRenderer.material.color = new Color( 1.0f, 0.0f, 0.0f, 0.5f );
		}

		return currentTargetSetpoint / maxSetpoint;
	}

	void OnEnable()
	{
		controlAxis = Configuration.GetSelectedAxis();

		moveBoxRenderer.transform.rotation = transform.rotation;
		initialMoveBoxPosition = transform.position - new Vector3( 0.0f, 0.99f * GetComponent<Collider>().bounds.extents.y, 0.0f );
		initialMoveBoxScale = moveBoxRenderer.transform.localScale;

		Debug.Log( string.Format( "Created collider {0} with position {1} size {2} and plane {3}", gameObject.name, initialPosition, size, moveBoxPlane.mesh.bounds.size ) );
	}

	public void EnableHelper()
	{
		helperEnabled = true;
		moveBoxRenderer.enabled = true;
	}

    void OnTriggerEnter( Collider collider )
    {
        //Debug.Log( "Trigger on " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 60.0f;
    }

    void OnTriggerExit( Collider collider )
    {
        //Debug.Log( "Trigger off " + collider.tag );
		if( controlAxis != null ) controlAxis.Stiffness = 60.0f;
    }
}
