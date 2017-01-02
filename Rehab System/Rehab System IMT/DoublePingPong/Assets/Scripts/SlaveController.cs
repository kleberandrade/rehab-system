using UnityEngine;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class SlaveController : Controller 
{
	const int POSITION = 0, VELOCITY = 1;

	void FixedUpdate()
	{
		float inputDelay = GameManager.GetConnection().GetNetworkDelay( elementID );

		Vector3 masterPosition = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, POSITION ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, POSITION ) );

		Vector3 masterVelocity = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, VELOCITY ),
											  0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, VELOCITY ) );

		Vector3 trackingError = masterPosition + masterVelocity * inputDelay - body.position;

		if( trackingError.magnitude > rangeLimits.magnitude / 2.0f ) body.MovePosition( masterPosition );
		else masterVelocity += trackingError;

		body.velocity = masterVelocity;
	}

	public void OnEnable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}
	public void OnDisable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}

	public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

		Physics.Raycast( body.position, body.velocity, out boundaryHit, 60.0f, layerMask );

		return boundaryHit.point;
	}
}