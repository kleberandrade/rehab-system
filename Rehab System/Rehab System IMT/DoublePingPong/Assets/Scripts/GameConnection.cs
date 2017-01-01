using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;


public abstract class GameConnection
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	protected ConnectionConfig connectionConfig = new ConnectionConfig();

	protected const int GAME_SERVER_PORT = 50004;
	protected const int PACKET_SIZE = 512;
	private const int PACKET_HEADER_LENGTH = sizeof(int);

	public const int TYPE_VALUES_NUMBER = 4;
	private const int VALUE_HEADER_SIZE = 2;
	private const int VALUE_DATA_SIZE = TYPE_VALUES_NUMBER * sizeof(float);
	protected const int VALUE_BLOCK_SIZE = VALUE_HEADER_SIZE + VALUE_DATA_SIZE;
	private const int VALUE_OID_OFFSET = 0, VALUE_INDEX_OFFSET = 1;

	protected int socketID = -1;
	protected int eventChannel = -1, dataChannel = -1;
	protected byte connectionError = 0;

	protected float networkDelay = 0.0f;

	protected byte[] inputBuffer = new byte[ PACKET_SIZE ];
	protected byte[] outputBuffer = new byte[ PACKET_SIZE ];
	private int lastInputPacketIndex = 0, outputPacketsCount = 0;

	protected Dictionary<KeyValuePair<byte,byte>,float[]> remoteValues = new Dictionary<KeyValuePair<byte,byte>,float[]>();
	protected Dictionary<KeyValuePair<byte,byte>,float[]> localValues = new Dictionary<KeyValuePair<byte,byte>,float[]>();
	private List<KeyValuePair<byte,byte>> updatedLocalKeys = new List<KeyValuePair<byte,byte>>();

	public GameConnection()
	{
		GlobalConfig networkConfig = new GlobalConfig();
		networkConfig.MaxPacketSize = PACKET_SIZE;
		NetworkTransport.Init( networkConfig );

		eventChannel = connectionConfig.AddChannel( QosType.Reliable );
		dataChannel = connectionConfig.AddChannel( QosType.StateUpdate ); // QosType.Unreliable sending just most recent
	}

	public static void Shutdown()
	{
		NetworkTransport.Shutdown();
	}

	public abstract void Connect();

	public void SetLocalValue( int objectID, int valueType, int valueIndex, float value ) 
	{
		KeyValuePair<byte,byte> localKey = new KeyValuePair<byte,byte>( (byte) objectID, (byte) valueType );

		if( !localValues.ContainsKey( localKey ) ) localValues[ localKey ] = new float[ TYPE_VALUES_NUMBER ];

		if( Math.Abs( localValues[ localKey ][ valueType ] - value ) > 0.1f )
		{
			localValues[ localKey ][ (int) valueType ] = value;
			updatedLocalKeys.Add( localKey );
		}
	}

	public float GetRemoteValue( int objectID, int valueType, int valueIndex )
	{
		KeyValuePair<byte,byte> remoteKey = new KeyValuePair<byte,byte>( (byte) objectID, (byte) valueType );

		if( remoteValues.ContainsKey( remoteKey ) ) return remoteValues[ remoteKey ][ valueType ];

		return 0.0f;
	}

	public void UpdateData( float updateTime )
	{
		int outputMessageLength = PACKET_HEADER_LENGTH;

		if( socketID == -1 ) return updateTime;

		foreach( KeyValuePair<byte,byte> localKey in updatedLocalKeys ) 
		{
			outputBuffer[ outputMessageLength + VALUE_OID_OFFSET ] = localKey.Key;
			outputBuffer[ outputMessageLength + VALUE_INDEX_OFFSET ] = localKey.Value;
			outputMessageLength += VALUE_HEADER_SIZE;

			for( int valueIndex = 0; valueIndex < TYPE_VALUES_NUMBER; valueIndex++ ) 
			{
				int dataOffset = outputMessageLength + valueIndex * sizeof(float);
				Buffer.BlockCopy( BitConverter.GetBytes( localValues[ localKey ][ valueIndex ] ), 0, outputBuffer, dataOffset, sizeof(float) );
			}

			outputMessageLength += VALUE_DATA_SIZE;
		}

		updatedLocalKeys.Clear();

		Buffer.BlockCopy( BitConverter.GetBytes( outputMessageLength ), 0, outputBuffer, 0, sizeof(int) );

		if( outputMessageLength > PACKET_HEADER_LENGTH ) SendUpdateMessage();

		if( ReceiveUpdateMessage() )
		{
			int inputMessageLength = Math.Min( BitConverter.ToInt32( inputBuffer, 0 ), InputAxisClient.BUFFER_SIZE - VALUE_BLOCK_SIZE );

			for( int dataOffset = PACKET_HEADER_LENGTH; dataOffset < inputMessageLength; dataOffset += VALUE_BLOCK_SIZE )
			{
				byte objectID = inputBuffer[ dataOffset + VALUE_OID_OFFSET ];
				byte axisIndex = inputBuffer[ dataOffset + VALUE_INDEX_OFFSET ];
				KeyValuePair<byte,byte> remoteKey = new KeyValuePair<byte,byte>( objectID, axisIndex );
				//Debug.Log( "Received values for key " + remoteKey.ToString() );
				if( !remoteValues.ContainsKey( remoteKey ) ) remoteValues[ remoteKey ] = new float[ TYPE_VALUES_NUMBER ];

				for( int valueIndex = 0; valueIndex < TYPE_VALUES_NUMBER; valueIndex++ )
					remoteValues[ remoteKey ][ valueIndex ] = BitConverter.ToSingle( inputBuffer, dataOffset + valueIndex * sizeof(float) );
			}
		}
	}

	protected abstract void SendUpdateMessage();

	protected abstract bool ReceiveUpdateMessage();

	public float GetNetworkDelay() { return networkDelay; }
}