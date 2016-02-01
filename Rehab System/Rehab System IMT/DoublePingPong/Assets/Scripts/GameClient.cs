using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameClient : MonoBehaviour
{
    protected const int DATA_SIZE = 2 * sizeof(byte) + sizeof(float);

    protected byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
    protected byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

    protected List<NetworkClient> remoteClients = new List<NetworkClient>();
    protected Dictionary<KeyValuePair<byte,byte>,float> localPositions = new Dictionary<KeyValuePair<byte,byte>,float>();
    protected Dictionary<KeyValuePair<byte,byte>,float> remotePositions = new Dictionary<KeyValuePair<byte,byte>,float>();

	public void Connect()
	{
		string gameServerHost = PlayerPrefs.GetString( ConnectionManager.GAME_SERVER_HOST_ID, /*ConnectionManager.LOCAL_SERVER_HOST*/"192.168.0.98" );
		ConnectionManager.GameClient.Connect( gameServerHost, 50004 );
	}

    public void SetLocalPosition( byte elementID, byte axisIndex, float value ) 
    { 
        localPositions[ new KeyValuePair<byte,byte>( elementID, axisIndex ) ] = value;
    }

    public bool HasRemoteKey( byte elementID, byte axisIndex )
    {
        return remotePositions.ContainsKey( new KeyValuePair<byte,byte>( elementID, axisIndex ) );
    }

    public float GetRemotePosition( byte elementID, byte axisIndex )
    {
        float value = 0.0f;

        remotePositions.TryGetValue( new KeyValuePair<byte,byte>( elementID, axisIndex ), out value );

        return value;
    }

    public KeyValuePair<byte,byte>[] GetRemotePositionKeys() 
    {
        return remotePositions.Keys.ToArray();
    }

	void Update()
	{
		int outputMessageLength = 1;

		foreach( KeyValuePair<byte,byte> localPositionKey in localPositions.Keys ) 
		{
			outputBuffer[ outputMessageLength ] = localPositionKey.Key;
			outputBuffer[ outputMessageLength + 1 ] = localPositionKey.Value;
			Buffer.BlockCopy( BitConverter.GetBytes( localPositions[ localPositionKey ] ), 0, outputBuffer, outputMessageLength + 2, sizeof(float) );

			outputMessageLength += DATA_SIZE;

            Debug.Log( "Sending " + localPositionKey.ToString() + " position: " + localPositions[ localPositionKey ].ToString() );
		}

		outputBuffer[ 0 ] = (byte) outputMessageLength;

		ConnectionManager.GameClient.SendData( outputBuffer );

		if( ConnectionManager.GameClient.ReceiveData( inputBuffer ) )
		{
			int inputMessageLength = (int) inputBuffer[ 0 ];

			for( int dataOffset = 1; dataOffset < inputMessageLength; dataOffset += DATA_SIZE )
			{
				KeyValuePair<byte,byte> remotePositionKey = new KeyValuePair<byte,byte>( inputBuffer[ dataOffset ], inputBuffer[ dataOffset + 1 ] );
				remotePositions[ remotePositionKey ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 );

                Debug.Log( "Received " + remotePositionKey.ToString() + " position: " + remotePositions[ remotePositionKey ].ToString() );
			}
		}
	}

    void OnApplicationQuit()
    {
        ConnectionManager.GameClient.Disconnect();
    }
}

