using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class BallController : Controller 
{
	const byte BALL_ID = 2;

	public float speed;	// Ball speed

	void FixedUpdate()
	{
        body.velocity *= speed / body.velocity.magnitude;

		UpdateMasterValues( body.position, body.velocity );
	}

    private void UpdateMasterValues( Vector3 newPosition, Vector3 newVelocity )
    {
		body.MovePosition( newPosition );
		body.velocity = newVelocity;

		gameConnection.SetLocalValue( BALL_ID, 0, NetworkValue.POSITION, body.position.x / rangeLimits.x );
		gameConnection.SetLocalValue( BALL_ID, 0, NetworkValue.VELOCITY, body.velocity.x / rangeLimits.x );
		gameConnection.SetLocalValue( BALL_ID, 2, NetworkValue.POSITION, body.position.z / rangeLimits.z );
		gameConnection.SetLocalValue( BALL_ID, 2, NetworkValue.VELOCITY, body.velocity.z / rangeLimits.z );
    }

    void OnTriggerExit( Collider collider )
	{
		if( collider.tag == "Boundary" ) UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), GenerateStartVector() * speed );
	}

    void OnTriggerEnter( Collider collider )
    {
		if( collider.tag == "Vertical" ) UpdateMasterValues( body.position, new Vector3( -body.velocity.x, 0.0f, body.velocity.z ) );
		else if( collider.tag == "Horizontal" ) UpdateMasterValues( body.position, new Vector3( body.velocity.x, 0.0f, -body.velocity.z ) );
    }

	Vector3 GenerateStartVector()
	{
		float rand = Random.Range( 0.0f, Mathf.PI * 2 );
		return new Vector3( Mathf.Cos( rand ), 0.0f, Mathf.Sin( rand ) ); 
	}

	public void OnEnable()
	{
		UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), GenerateStartVector() * speed );
	}
	public void OnDisable()
	{
		UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), Vector3.zero );
	}

}
