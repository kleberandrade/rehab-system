using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public class TargetController : Controller 
{
    public bool isPlaying = false;

	private float targetPositionX = 0.0f, targetPositionZ = 0.0f;

	void FixedUpdate()
	{
        if( isPlaying )
        {
			float masterBallPositionX = gameConnection.GetRemoteValue( (byte) Movable.BALL, 2, NetworkValue.POSITION ) * rangeLimits.x;
			float masterBallPositionZ = gameConnection.GetRemoteValue( (byte) Movable.BALL, 0, NetworkValue.POSITION ) * rangeLimits.z;

			if( targetPositionX != masterBallPositionX || targetPositionZ != masterBallPositionZ ) 
			{
				float masterBallVelocityX = gameConnection.GetRemoteValue( (byte) Movable.BALL, 2, NetworkValue.VELOCITY ) * rangeLimits.x;
				float masterBallVelocityZ = gameConnection.GetRemoteValue( (byte) Movable.BALL, 0, NetworkValue.VELOCITY ) * rangeLimits.z;

				float ballFollowingErrorX = masterBallPositionX - body.position.x;
				float ballFollowingErrorZ = masterBallPositionZ - body.position.z;

				if( Mathf.Abs( ballFollowingErrorX ) < rangeLimits.x / 2 && Mathf.Abs( ballFollowingErrorZ ) < rangeLimits.z / 2 ) 
				{

					masterBallVelocityX += ballFollowingErrorX;
				    masterBallVelocityZ += ballFollowingErrorZ;

					body.velocity = new Vector3( masterBallVelocityX, 0.0f, masterBallVelocityZ );
				} 
				else 
				{
					body.MovePosition( new Vector3( masterBallPositionX, body.position.y, masterBallPositionZ ) );

					body.velocity = new Vector3( masterBallVelocityX, 0.0f, masterBallVelocityZ );
				}

				targetPositionX = masterBallPositionX;
				targetPositionZ = masterBallPositionZ;
			}

			//File.AppendAllText( logFileName, Time.realtimeSinceStartup.ToString() + "\t" 
			//	+ ball.position.x.ToString() + "\t" + ball.position.z.ToString() + System.Environment.NewLine );
		}
			
	}

    void OnTriggerExit( Collider collider )
	{
        if( collider.tag == "Boundary" ) 
		{
			body.position = new Vector3( 0.0f, body.position.y, 0.0f );
			body.velocity = Vector3.zero;
		}
	}

    void OnTriggerEnter( Collider collider )
    {
		if( collider.tag == "Vertical" ) body.velocity = new Vector3( -body.velocity.x, 0.0f, body.velocity.z );
		else if( collider.tag == "Horizontal" ) body.velocity = new Vector3( body.velocity.x, 0.0f, -body.velocity.z );
    }

    public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

		Physics.Raycast( body.position, body.velocity, out boundaryHit, 60f, layerMask );
		
        return boundaryHit.point;
	}

	public void OnEnable()
	{
		isPlaying = true;
		body.position = new Vector3( 0.0f, body.position.y, 0.0f );
		body.velocity = Vector3.zero;
	}
	public void OnDisable()
	{
		isPlaying = false;
		body.position = new Vector3( 0.0f, body.position.y, 0.0f );
		body.velocity = Vector3.zero;
	}

}
