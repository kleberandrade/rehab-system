using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(Rigidbody) ) ]
[ RequireComponent( typeof(BoxCollider) ) ]
public class SlaveController : Controller 
{
    void FixedUpdate()
    {
        // Get remotely controlled object position (z) and set it locally (x)
		float remoteInput = gameConnection.GetRemoteValue( (byte) elementID, 0, NetworkValue.POSITION );

        float remotePosition = remoteInput * rangeLimits.x;

        //File.AppendAllText( textFile, Time.realtimeSinceStartup.ToString() + "\t" + remotePosition.ToString() + Environment.NewLine );

        body.MovePosition( new Vector3( Mathf.Clamp( remotePosition, -rangeLimits.x, rangeLimits.x ), 0.0f, body.position.z ) );
    }
}

