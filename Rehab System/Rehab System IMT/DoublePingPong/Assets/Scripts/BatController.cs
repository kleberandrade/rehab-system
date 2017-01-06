using UnityEngine;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class BatController : Controller 
{
	const int POSITION = 0, VELOCITY = 1;

	void FixedUpdate()
	{
		float inputDelay = GameManager.GetConnection().GetNetworkDelay( elementID );

		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, VELOCITY ) );

		//Debug.Log( "element " + elementID.ToString() + " position: " + masterPosition.ToString() + " - velocity: " + masterVelocity.ToString() );
		body.MovePosition( masterPosition + masterVelocity * inputDelay );
		body.velocity = masterVelocity;

		// Send locally controlled object position over network
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, POSITION, body.position.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, POSITION, body.position.z );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, VELOCITY, body.velocity.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, VELOCITY, body.velocity.z );
	}
}