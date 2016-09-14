using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameServer : GameConnection
{
    void Start()
    {
		GlobalConfig networkConfig = new GlobalConfig();
		networkConfig.MaxPacketSize = PACKET_SIZE;
		NetworkTransport.Init( networkConfig );

		ConnectionConfig connectionConfig = new ConnectionConfig();
		HostTopology networkTopology = new HostTopology( connectionConfig, 10 );
		broadcastHostID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );
    }

	void FixedUpdate()
	{
		int outputMessageLength = 1;

        foreach( KeyValuePair<byte,byte> localKey in localValues.Keys ) 
		{
            if( localValuesUpdated[ localKey ] )
            {
    			outputBuffer[ outputMessageLength ] = localKey.Key;
    			outputBuffer[ outputMessageLength + 1 ] = localKey.Value;
    			Buffer.BlockCopy( BitConverter.GetBytes( localValues[ localKey ][ 0 ] ), 0, outputBuffer, outputMessageLength + 2, sizeof(float) );
                Buffer.BlockCopy( BitConverter.GetBytes( localValues[ localKey ][ 1 ] ), 0, outputBuffer, outputMessageLength + 2 + sizeof(float), sizeof(float) );
                Buffer.BlockCopy( BitConverter.GetBytes( localValues[ localKey ][ 2 ] ), 0, outputBuffer, outputMessageLength + 2 + 2 * sizeof(float), sizeof(float) );

    			outputMessageLength += DATA_SIZE;

                localValuesUpdated[ localKey ] = false;
            }

            //Debug.Log( "Sending " + localKey.ToString() + " position: " + localValues[ localKey ].ToString() );
		}

		outputBuffer[ 0 ] = (byte) outputMessageLength;

		int remoteHostID, remoteConnectionID, remoteClientID, receivedSize;
		if( NetworkTransport.Receive( out remoteHostID, out remoteConnectionID, out remoteClientID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.DataEvent )
		{
			if( connectionError != (byte) NetworkError.Ok )
			{
	            int inputMessageLength = Math.Min( (int) inputBuffer[ 0 ], NetworkInterface.BUFFER_SIZE - DATA_SIZE );

	            for( int dataOffset = 1; dataOffset < inputMessageLength; dataOffset += DATA_SIZE )
				{
					KeyValuePair<byte,byte> remoteKey = new KeyValuePair<byte,byte>( inputBuffer[ dataOffset ], inputBuffer[ dataOffset + 1 ] );
	                Debug.Log( "Received values for key " + remoteKey.ToString() );
	                if( !remoteValues.ContainsKey( remoteKey ) ) remoteValues[ remoteKey ] = new float[ 3 ];
	                    
	    			remoteValues[ remoteKey ][ 0 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 );
	                remoteValues[ remoteKey ][ 1 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 + sizeof(float) );
	                remoteValues[ remoteKey ][ 2 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 + 2 * sizeof(float) );

	                //remoteValues[ remoteKey ][ (int) NetworkValue.POSITION ] += remoteValues[ remoteKey ][ (int) NetworkValue.VELOCITY ] * Time.fixedDeltaTime;

	                //Debug.Log( "Received axis " + ( dataOffset / DATA_SIZE ).ToString() + ": " + remoteKey.ToString() + ": " + remoteValues[ remoteKey ].ToString() );
				}

				if( outputMessageLength > 1 ) NetworkTransport.Send( localHostID, remoteConnectionID, remoteClientID, outputBuffer, PACKET_SIZE, out connectionError );
			}
		}
            
	}
}

