using UnityEngine;
using System.Collections;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(MeshCollider) ) ]
public class BallController : MonoBehaviour 
{

	public float speed;	// Ball speed

    private Rigidbody ball;		// Ball rigid body

    public Collider boundaries;
    private Vector3 rangeLimits = new Vector3( 7.25f, 0.0f, 7.25f );

    public bool isPlaying = false;

    public GameClient gameClient;
    public bool isMaster = true;

	void Awake()
    {
		ball = GetComponent<Rigidbody>();
	}

	void Start () 
	{
        ball.angularVelocity = Vector3.zero;
        ball.velocity = Vector3.zero;

        rangeLimits = boundaries.bounds.extents;
	}

	void FixedUpdate()
	{
        if( isPlaying )
        {
            if( isMaster )
            {
                ball.velocity *= speed / ball.velocity.magnitude;

                UpdateMasterValues( ball.position, ball.velocity );
            } 
            else
            {
                float masterBallPositionX = gameClient.GetremoteValue( (byte) Movable.BALL, 2, NetworkValue.POSITION ) * rangeLimits.x;
                float masterBallPositionZ = gameClient.GetremoteValue( (byte) Movable.BALL, 0, NetworkValue.POSITION ) * rangeLimits.z;

                float masterBallVelocityX = gameClient.GetremoteValue( (byte) Movable.BALL, 2, NetworkValue.VELOCITY ) * rangeLimits.x;
                float masterBallVelocityZ = gameClient.GetremoteValue( (byte) Movable.BALL, 0, NetworkValue.VELOCITY ) * rangeLimits.z;

                Debug.Log( "Remote position: " + masterBallPositionX.ToString() + "," + masterBallPositionZ.ToString() );

                if( Mathf.Abs( masterBallPositionX - ball.position.x ) < rangeLimits.x && Mathf.Abs( masterBallPositionZ - ball.position.z ) < rangeLimits.z )
                {
                    masterBallVelocityX += ( masterBallPositionX - ball.position.x ) / Time.fixedDeltaTime;
                    masterBallVelocityZ += ( masterBallPositionZ - ball.position.z ) / Time.fixedDeltaTime;

                    ball.MovePosition( new Vector3( ( masterBallPositionX + ball.position.x ) / 2, ball.position.y, ( masterBallPositionZ + ball.position.z ) / 2 ) );

                    ball.velocity = new Vector3( masterBallVelocityX, 0.0f, masterBallVelocityZ );
                }
                else
                {
                    ball.MovePosition( new Vector3( masterBallPositionX, ball.position.y, masterBallPositionZ ) );

                    ball.velocity = new Vector3( masterBallVelocityX, 0.0f, masterBallVelocityZ );
                }
            }

        } 
        else if( gameClient.HasRemoteKey( (byte) Movable.BALL, 0 ) )
        {
            isMaster = false;                
        }
	}

    private void UpdateMasterValues( Vector3 newPosition, Vector3 newVelocity )
    {
        if( isMaster )
        {
            ball.MovePosition( newPosition );
            ball.velocity = newVelocity;

            gameClient.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.POSITION, ball.position.x / rangeLimits.x );
            gameClient.SetLocalValue( (byte) Movable.BALL, 0, NetworkValue.VELOCITY, ball.velocity.x / rangeLimits.x );
            gameClient.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.POSITION, ball.position.z / rangeLimits.z );
            gameClient.SetLocalValue( (byte) Movable.BALL, 2, NetworkValue.VELOCITY, ball.velocity.z / rangeLimits.z );
        }
    }

    void OnTriggerExit( Collider collider )
	{
        if( collider.tag == "Boundary" ) UpdateMasterValues( new Vector3( 0.0f, ball.position.y, 0.0f ), RandVectOnGround() * speed );
	}

    void OnTriggerEnter( Collider collider )
    {
        if( collider.tag == "Vertical" ) UpdateMasterValues( ball.position, new Vector3( -ball.velocity.x, 0.0f, ball.velocity.z ) );
        else if( collider.tag == "Horizontal" ) UpdateMasterValues( ball.position, new Vector3( ball.velocity.x, 0.0f, -ball.velocity.z ) );
    }

    public Vector3 FindImpactPoint( int layerMask )
	{
		RaycastHit boundaryHit;

        Physics.Raycast( ball.position, ball.velocity, out boundaryHit, 60f, layerMask );
		
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
        UpdateMasterValues( new Vector3( 0.0f, ball.position.y, 0.0f ), RandVectOnGround() * speed );
	}
	public void StopPlay()
	{
		isPlaying = false;
        UpdateMasterValues( new Vector3( 0.0f, ball.position.y, 0.0f ), Vector3.zero );
	}

}
