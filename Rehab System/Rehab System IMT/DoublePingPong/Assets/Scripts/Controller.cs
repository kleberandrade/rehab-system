using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(Collider) ) ]
public abstract class Controller : MonoBehaviour 
{
    public int elementID;

	public Collider boundaries;
    protected Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );
	protected Vector3 initialPosition = Vector3.zero;

	protected string textFile;

	protected Rigidbody body;
	protected Collider col;

	protected GameConnection gameConnection;

	void Awake()
	{
		body = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();

		//Debug.Log( string.Format( "Awake() called for {0}. Body: {1} - Collider: {2}", gameObject.name, body, col ) );

		rangeLimits = boundaries.bounds.extents - Vector3.one * col.bounds.extents.magnitude;
		initialPosition = transform.position;

		// Start file for record movements
        //textFile = "./LogFilePlayer" + GetInstanceID().ToString() + ".txt";
		//if( File.Exists( textFile ) ) File.Delete( textFile );
	}

	void Start()
	{
		gameConnection = GameManager.GetConnection();
	}
}
