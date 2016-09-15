using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(MeshCollider) ) ]
public class MasterController : Controller 
{
	public float speed;	// Ball speed

    public bool isPlaying = false;

    public bool isMaster = true;

    private string logFileName;

	private float targetPositionX = 0.0f, targetPositionZ = 0.0f;

	void FixedUpdate()
	{
        if( isPlaying )
        {
			if( isMaster ) 
			{
				body.velocity *= speed / body.velocity.magnitude;

				UpdateMasterValues( body.position, body.velocity );
			} 
			else 
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
				//	+ body.position.x.ToString() + "\t" + body.position.z.ToString() + System.Environment.NewLine );
			}

        } 
		else if( gameConnection.HasRemoteKey( (byte) Movable.BALL, 0 ) )
        {
            isMaster = false;                
        }
	}

    protected void UpdateMasterValues( Vector3 newPosition, Vector3 newVelocity )
    {
		body.MovePosition( newPosition );
		body.velocity = newVelocity;

        if( isMaster )
        {
			gameConnection.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.POSITION, body.position.x / rangeLimits.x );
			gameConnection.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.VELOCITY, body.velocity.x / rangeLimits.x );
			gameConnection.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.POSITION, body.position.z / rangeLimits.z );
			gameConnection.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.VELOCITY, body.velocity.z / rangeLimits.z );
        }
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

    public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

		Physics.Raycast( body.position, body.velocity, out boundaryHit, 60f, layerMask );
		
        return boundaryHit.point;
	}

	Vector3 GenerateStartVector()
	{
		float rand = Random.Range( 0.0f, Mathf.PI * 2 );
		return new Vector3( Mathf.Cos( rand ), 0.0f, Mathf.Sin( rand ) ); 
	}

	public void StartPlay()
	{
		isPlaying = true;
		UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), GenerateStartVector() * speed );
	}
	public void StopPlay()
	{
		isPlaying = false;
		UpdateMasterValues( new Vector3( 0.0f, body.position.y, 0.0f ), Vector3.zero );
	}

}
