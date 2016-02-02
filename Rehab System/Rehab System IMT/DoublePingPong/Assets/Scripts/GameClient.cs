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

    void Start()
    {
        Connect();
    }

	public void Connect()
	{
		string gameServerHost = PlayerPrefs.GetString( ConnectionManager.GAME_SERVER_HOST_ID, /*ConnectionManager.LOCAL_SERVER_HOST*/"192.168.0.98" );
		ConnectionManager.GameClient.Connect( gameServerHost, 50004 );
	}

    public void SetLocalPosition( byte elementID, byte axisIndex, float value ) 
    {
        if( localPositions.ContainsKey( new KeyValuePair<byte,byte>( elementID, axisIndex ) ) )
        {
            if( Mathf.Abs( localPositions[ new KeyValuePair<byte,byte>( elementID, axisIndex ) ] - value ) > 0.5 )
                localPositions[ new KeyValuePair<byte,byte>( elementID, axisIndex ) ] = value;
        }
        else
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

	void FixedUpdate()
	{
		int outputMessageLength = 1;

		foreach( KeyValuePair<byte,byte> localPositionKey in localPositions.Keys ) 
		{
            if( float.IsNaN( localPositions[ localPositionKey ] ) )
            {
    			outputBuffer[ outputMessageLength ] = localPositionKey.Key;
    			outputBuffer[ outputMessageLength + 1 ] = localPositionKey.Value;
    			Buffer.BlockCopy( BitConverter.GetBytes( localPositions[ localPositionKey ] ), 0, outputBuffer, outputMessageLength + 2, sizeof(float) );

    			outputMessageLength += DATA_SIZE;

                localPositions[ localPositionKey ] = float.NaN;
            }

            //Debug.Log( "Sending " + localPositionKey.ToString() + " position: " + localPositions[ localPositionKey ].ToString() );
		}

		outputBuffer[ 0 ] = (byte) outputMessageLength;

        if( outputMessageLength > 1 ) ConnectionManager.GameClient.SendData( outputBuffer );

		if( ConnectionManager.GameClient.ReceiveData( inputBuffer ) )
		{
			int inputMessageLength = (int) inputBuffer[ 0 ];

            //Debug.Log( "Received " + inputMessageLength.ToString() + " bytes" );

			for( int dataOffset = 1; dataOffset < inputMessageLength; dataOffset += DATA_SIZE )
			{
				KeyValuePair<byte,byte> remotePositionKey = new KeyValuePair<byte,byte>( inputBuffer[ dataOffset ], inputBuffer[ dataOffset + 1 ] );
				remotePositions[ remotePositionKey ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 );

                //Debug.Log( "Received axis " + ( dataOffset / DATA_SIZE ).ToString() + ": " + remotePositionKey.ToString() + ": " + remotePositions[ remotePositionKey ].ToString() );
			}
		}
	}

    void OnApplicationQuit()
    {
        ConnectionManager.GameClient.Disconnect();
    }
}

