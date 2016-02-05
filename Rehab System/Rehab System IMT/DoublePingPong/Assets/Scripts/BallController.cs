using UnityEngine;
using System.Collections;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(MeshCollider) ) ]
public class BallController : MonoBehaviour 
{

	public float speed;	// Ball speed

    private Rigidbody body;		// Ball rigid body
    private SphereCollider ballShape;

    public BoxCollider boundaries;
    private Vector3 rangeLimits = new Vector3( 7.25f, 0.0f, 7.25f );

    public bool isPlaying = false;

    public GameClient gameClient;
    public bool isMaster = true;

	void Awake()
    {
		body = GetComponent<Rigidbody>();
        ballShape = GetComponent<SphereCollider>();
	}

	void Start () 
	{
		body.velocity = RandVectOnGround() * speed;	// Inicialize with a random velocity
//		body.velocity = 0.2f * Vector3.down + 0.1f * Vector3.one;

        rangeLimits = boundaries.bounds.extents;
	}

	void FixedUpdate()
	{
		if( Mathf.Abs( body.velocity.y ) < Mathf.Epsilon )
		{
			if( isPlaying )
			{
                if( isMaster )
                {
                    if( Mathf.Abs( body.velocity.magnitude ) < Mathf.Epsilon ) OnTriggerExit( boundaries );

                    gameClient.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.POSITION, body.position.x );
                    gameClient.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.VELOCITY, body.velocity.x );
                    gameClient.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.POSITION, body.position.z );
                    gameClient.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.VELOCITY, body.velocity.z );
                }
				else
				{
                    body.MovePosition( new Vector3( Mathf.Clamp( gameClient.GetremoteValue( (byte) Movable.BALL, 0, NetworkValue.POSITION ), -rangeLimits.x, rangeLimits.x ),
                                       0.0f, Mathf.Clamp( gameClient.GetremoteValue( (byte) Movable.BALL, 2, NetworkValue.POSITION ), -rangeLimits.z, rangeLimits.z ) ) );
				}

			}
			else 
			{
                if( gameClient.HasRemoteKey( (byte) Movable.BALL, 0 ) ) isMaster = false;

				body.angularVelocity = Vector3.zero;
				body.velocity = Vector3.zero;
			}
		}
	}

	void OnTriggerExit( Collider boundaries )
	{
        body.MovePosition( new Vector3( 0.0f, 10.0f, 0.0f ) );
        body.velocity = RandVectOnGround() * speed;
	}

    public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

        Physics.Raycast( body.position, body.velocity, out boundaryHit, 60f, layerMask );
		
        return boundaryHit.point;
	}

	Vector3 RandVectOnGround()
	{
		float rand = Random.Range( 0.0f, Mathf.PI * 2 );
		return new Vector3( Mathf.Cos( rand ), 0.0f, Mathf.Sin( rand ) ); 
	}

	public void StartPlay()
	{
		isPlaying = true;
        OnTriggerExit( boundaries );
	}
	public void StopPlay()
	{
		isPlaying = false;
        OnTriggerExit( boundaries );
	}

}
