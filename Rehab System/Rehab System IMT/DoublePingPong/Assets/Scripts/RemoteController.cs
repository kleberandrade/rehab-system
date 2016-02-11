using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class RemoteController : MonoBehaviour 
{
    public int elementID;

    public Collider boundaries;
    private Vector3 rangeLimits = new Vector3( 7.5f, 0.0f, 7.5f );

    private string textFile;

    private Rigidbody remoteBody;
    private BoxCollider remoteCollider;

    public GameClient gameClient;

    void Start()
    {
        remoteBody = GetComponent<Rigidbody>();
        remoteCollider = GetComponent<BoxCollider>();

        rangeLimits = boundaries.bounds.extents - remoteCollider.bounds.extents;

        // Start file for record movements
        textFile = "./LogFileRemote" + GetInstanceID().ToString() + ".txt";
        if( File.Exists( textFile ) ) File.Delete( textFile );
    }

    void FixedUpdate()
    {
        // Get remotely controlled object position (z) and set it locally (x)
        float remoteInput = gameClient.GetremoteValue( (byte) Movable.WALL, 0, NetworkValue.POSITION );

        float remotePosition = remoteInput * rangeLimits.x;

        File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" 
                                     + Mathf.Clamp( remotePosition, -rangeLimits.x, rangeLimits.x ).ToString() + Environment.NewLine );

        remoteBody.MovePosition( new Vector3( Mathf.Clamp( remotePosition, -rangeLimits.x, rangeLimits.x ), 0.0f, remoteBody.position.z ) );
    }
}

