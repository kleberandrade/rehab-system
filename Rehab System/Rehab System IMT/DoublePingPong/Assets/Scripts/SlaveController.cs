using UnityEngine;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class SlaveController : PositionController 
{
	public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

		Physics.Raycast( body.position, body.velocity, out boundaryHit, 60.0f, layerMask );

		return boundaryHit.point;
	}
}