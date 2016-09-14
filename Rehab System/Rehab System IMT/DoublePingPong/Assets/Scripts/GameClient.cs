using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public enum NetworkValue { POSITION, VELOCITY, FORCE };

public class GameClient : MonoBehaviour
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	private const int GAME_SERVER_PORT = 50004;
	private const int PACKET_SIZE = 512;
    private const int DATA_SIZE = 2 * sizeof(byte) + 3 * sizeof(float);

	private int localHostID, connectionID, clientID;
	private byte connectionError = 0;

	private byte[] inputBuffer = new byte[ PACKET_SIZE ];
	private byte[] outputBuffer = new byte[ PACKET_SIZE ];

    private Dictionary<KeyValuePair<byte,byte>, float[]> localValues = new Dictionary<KeyValuePair<byte,byte>, float[]>();
    private Dictionary<KeyValuePair<byte,byte>, bool> localValuesUpdated = new Dictionary<KeyValuePair<byte,byte>, bool>();
    private Dictionary<KeyValuePair<byte,byte>, float[]> remoteValues = new Dictionary<KeyValuePair<byte,byte>, float[]>();

    void Start()
    {
		GlobalConfig networkConfig = new GlobalConfig();
		networkConfig.MaxPacketSize = PACKET_SIZE;
		NetworkTransport.Init( networkConfig );

		ConnectionConfig connectionConfig = new ConnectionConfig();
		clientID = connectionConfig.AddChannel( QosType.StateUpdate  ); // QosType.Unreliable sending just most recent

		HostTopology networkTopology = new HostTopology( connectionConfig, 1 );
		localHostID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );

		string gameServerHost = PlayerPrefs.GetString( GAME_SERVER_HOST_ID, "localhost" );
		connectionID = NetworkTransport.Connect( localHostID, gameServerHost, GAME_SERVER_PORT, 0, out connectionError );
    }

    public void SetLocalValue( byte elementID, byte axisIndex, NetworkValue valueType, float value ) 
    {
        KeyValuePair<byte,byte> localKey = new KeyValuePair<byte,byte>( elementID, axisIndex );

        if( !localValues.ContainsKey( localKey ) ) 
        {
            localValuesUpdated[ localKey ] = true;
            localValues[ localKey ] = new float[ 3 ];
        }

        if( Mathf.Abs( localValues[ localKey ][ (int) valueType ] - value ) > 0.1f )
        {
          localValues[ localKey ][ (int) valueType ] = value;
          localValuesUpdated[ localKey ] = true;

          Debug.Log( "Setting " + localKey.ToString() + " position" );
        }
    }

    public bool HasRemoteKey( byte elementID, byte axisIndex )
    {
        return remoteValues.ContainsKey( new KeyValuePair<byte,byte>( elementID, axisIndex ) );
    }

    public float GetRemoteValue( byte elementID, byte axisIndex, NetworkValue valueType )
    {
        float[] values;

        if( remoteValues.TryGetValue( new KeyValuePair<byte,byte>( elementID, axisIndex ), out values ) ) 
            return values[ (int) valueType ];

        return 0.0f;
    }

    public KeyValuePair<byte,byte>[] GetRemoteKeys() 
    {
        return remoteValues.Keys.ToArray();
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

		if( outputMessageLength > 1 ) NetworkTransport.Send( localHostID, connectionID, clientID, outputBuffer, PACKET_SIZE, out connectionError );

		int remoteHostID, remoteConnectionID, remoteServerID, receivedSize;
		if( NetworkTransport.Receive( out remoteHostID, out remoteConnectionID, out remoteServerID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.DataEvent )
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
			}
		}       
	}

	void OnApplicationQuit()
	{
		NetworkTransport.Shutdown();
	}
}

