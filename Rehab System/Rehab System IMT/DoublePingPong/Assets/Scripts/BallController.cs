using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class BallController : Controller 
{
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

		gameConnection = GameManager.GetConnection();
		gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.X, NetworkValue.POSITION, body.position.x );
		gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.X, NetworkValue.VELOCITY, body.velocity.x );
		gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.Z, NetworkValue.POSITION, body.position.z );
		gameConnection.SetLocalValue( (byte) elementID, NetworkAxis.Z, NetworkValue.VELOCITY, body.velocity.z );
    }

    void OnTriggerExit( Collider collider )
	{
		if( collider.tag == "Boundary" ) UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), GenerateStartVector() * speed );
	}

    void OnTriggerEnter( Collider collider )
    {
		Debug.Log( "Colliding with " + collider.tag + " on layer " + collider.gameObject.layer );

		if( collider.tag == "Vertical" ) UpdateMasterValues( body.position, new Vector3( -body.velocity.x, 0.0f, body.velocity.z ) );
		else if( collider.tag == "Horizontal" ) UpdateMasterValues( body.position, new Vector3( body.velocity.x, 0.0f, -body.velocity.z ) );
		else if( collider.tag == "Tower" ) UpdateMasterValues( body.position, new Vector3( -body.velocity.x, 0.0f, -body.velocity.z ) );
    }

	Vector3 GenerateStartVector()
	{
		float rand = Random.Range( 0.0f, Mathf.PI * 2 );
		return new Vector3( Mathf.Cos( rand ), 0.0f, Mathf.Sin( rand ) ); 
	}

	public void OnEnable()
	{
		UpdateMasterValues( initialPosition, GenerateStartVector() * speed );
	}

	public void OnDisable()
	{
		UpdateMasterValues( initialPosition, Vector3.zero );
	}

}
