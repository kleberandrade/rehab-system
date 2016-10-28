using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public abstract class Controller : MonoBehaviour 
{
    public int elementID;

	public Collider boundaries;
    protected Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );
	protected Vector3 initialPosition = Vector3.zero;

	protected Rigidbody body;

	protected GameConnection gameConnection;

	void Awake()
	{
		body = GetComponent<Rigidbody>();

		body.velocity = Vector3.zero;

		//Debug.Log( string.Format( "Awake() called for {0}. Body: {1} - Collider: {2}", gameObject.name, body, col ) );

		rangeLimits = boundaries.bounds.extents - Vector3.one * GetComponent<Collider>().bounds.extents.magnitude;
		initialPosition = transform.position;
	}

	void Start()
	{
		gameConnection = GameManager.GetConnection();
	}
}
