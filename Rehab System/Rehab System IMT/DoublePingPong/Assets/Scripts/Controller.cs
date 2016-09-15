using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public abstract class Controller : MonoBehaviour 
{
    public int elementID;

	public Collider boundaries;
    protected Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );

	protected string textFile;

	protected Rigidbody body;
	protected BoxCollider collider;

	public GameObject connectionManager;
	protected GameConnection gameConnection;

	void Start()
	{
        body = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();

		if( Gameplay.isServer ) gameConnection = connectionManager.GetComponent<GameServer>();
		else gameConnection = connectionManager.GetComponent<GameClient>();

        rangeLimits = boundaries.bounds.extents - collider.bounds.extents;

		// Start file for record movements
        //textFile = "./LogFilePlayer" + GetInstanceID().ToString() + ".txt";
		//if( File.Exists( textFile ) ) File.Delete( textFile );
	}      

	public abstract float ControlPosition( Vector3 target, out float error );
}
