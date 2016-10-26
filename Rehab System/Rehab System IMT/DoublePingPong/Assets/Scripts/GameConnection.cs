using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public enum NetworkValue : int { POSITION, VELOCITY, FORCE };
public enum NetworkAxis : byte { X, Y, Z };

public abstract class GameConnection
{
	protected const int GAME_SERVER_PORT = 50004;
	protected const int PACKET_SIZE = 512;
	protected const int DATA_SIZE = 2 * sizeof(byte) + 3 * sizeof(float);

	protected const int BROADCAST_KEY = 1000;
	protected const int BROADCAST_VERSION = 1, BROADCAST_SUBVERSION = 1;

	protected int socketID = -1;
	protected int eventChannel = -1, dataChannel = -1;
	protected byte connectionError = 0;

	protected byte[] inputBuffer = new byte[ PACKET_SIZE ];
	protected byte[] outputBuffer = new byte[ PACKET_SIZE ];

	protected Dictionary<KeyValuePair<byte,byte>, float[]> localValues = new Dictionary<KeyValuePair<byte,byte>, float[]>();
	protected Dictionary<KeyValuePair<byte,byte>, bool> localValuesUpdated = new Dictionary<KeyValuePair<byte,byte>, bool>();
	protected Dictionary<KeyValuePair<byte,byte>, float[]> remoteValues = new Dictionary<KeyValuePair<byte,byte>, float[]>();

	public GameConnection()
	{
		GlobalConfig networkConfig = new GlobalConfig();
		networkConfig.MaxPacketSize = PACKET_SIZE;
		NetworkTransport.Init( networkConfig );

		ConnectionConfig connectionConfig = new ConnectionConfig();
		eventChannel = connectionConfig.AddChannel( QosType.Reliable );
		dataChannel = connectionConfig.AddChannel( QosType.StateUpdate ); // QosType.Unreliable sending just most recent

		Connect( connectionConfig );
	}

	public static void Shutdown()
	{
		NetworkTransport.Shutdown();
	}

	protected abstract void Connect( ConnectionConfig connectionConfig );

	public void SetLocalValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType, float value ) 
    {
		KeyValuePair<byte,byte> localKey = new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex );

        if( !localValues.ContainsKey( localKey ) ) 
        {
            localValuesUpdated[ localKey ] = true;
            localValues[ localKey ] = new float[ 3 ];
        }

        if( Mathf.Abs( localValues[ localKey ][ (int) valueType ] - value ) > 0.1f )
        {
		  	localValues[ localKey ][ (int) valueType ] = value;
          	localValuesUpdated[ localKey ] = true;

          	//Debug.Log( "Setting " + localKey.ToString() + " position" );
        }
    }

	public bool HasRemoteKey( int elementID, NetworkAxis axisIndex )
    {
		return remoteValues.ContainsKey( new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex ) );
    }

	public float GetRemoteValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType )
    {
        float[] values;

		if( remoteValues.TryGetValue( new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex ), out values ) ) 
            return values[ (int) valueType ];

        return 0.0f;
    }

//    public KeyValuePair<byte,byte>[] GetRemoteKeys() 
//    {
//        return remoteValues.Keys.ToArray();
//    }

	public float UpdateData()
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

		if( outputMessageLength > 1 ) SendUpdateMessage();

		if( ReceiveUpdateMessage() )
		{
			int inputMessageLength = Math.Min( (int) inputBuffer[ 0 ], AxisClient.BUFFER_SIZE - DATA_SIZE );

	        for( int dataOffset = 1; dataOffset < inputMessageLength; dataOffset += DATA_SIZE )
			{
				KeyValuePair<byte,byte> remoteKey = new KeyValuePair<byte,byte>( inputBuffer[ dataOffset ], inputBuffer[ dataOffset + 1 ] );
	            //Debug.Log( "Received values for key " + remoteKey.ToString() );
	            if( !remoteValues.ContainsKey( remoteKey ) ) remoteValues[ remoteKey ] = new float[ 3 ];
	                    
	    		remoteValues[ remoteKey ][ 0 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 );
	            remoteValues[ remoteKey ][ 1 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 + sizeof(float) );
	            remoteValues[ remoteKey ][ 2 ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 + 2 * sizeof(float) );

	            //remoteValues[ remoteKey ][ (int) NetworkValue.POSITION ] += remoteValues[ remoteKey ][ (int) NetworkValue.VELOCITY ] * Time.fixedDeltaTime;

	            //Debug.Log( "Received axis " + ( dataOffset / DATA_SIZE ).ToString() + ": " + remoteKey.ToString() + ": " + remoteValues[ remoteKey ].ToString() );
			}
		}

		return GetNetworkDelay();
	}

	protected abstract void SendUpdateMessage();

	protected abstract bool ReceiveUpdateMessage();

	protected abstract float GetNetworkDelay();
}

