using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public enum NetworkAxis : byte { X, Y, Z };

public struct ConnectionInfo
{
	public int socketID, connectionID, channel;
	public float sendRate, receiveRate;
	public int rtt, ioTime, lostPackets;
}

public abstract class GameConnectionBase
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	protected ConnectionConfig connectionConfig = new ConnectionConfig();

	protected const int GAME_SERVER_PORT = 50004;
	protected const int PACKET_SIZE = 512;
	protected const int DATA_SIZE = 2 * sizeof(byte) + 3 * sizeof(float);

	protected int socketID = -1;
	protected int eventChannel = -1, dataChannel = -1;
	protected byte connectionError = 0;

	protected byte[] inputBuffer = new byte[ PACKET_SIZE ];
	protected byte[] outputBuffer = new byte[ PACKET_SIZE ];

	public GameConnectionBase()
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

	public abstract void SetLocalValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType, float value );
	public abstract bool HasRemoteKey( int elementID, NetworkAxis axisIndex );
	public abstract float GetRemoteValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType );

	public abstract float UpdateData( float updateTime );

	public abstract int GetNetworkDelay();
}

public abstract class GameConnection<CompensatorType> : GameConnectionBase where CompensatorType : NetworkCompensator, new()
{
	public class NetworkOperator 
	{
		public float[] outputValues = new float[ (int) NetworkValue.VALUES_NUMBER ];
		public bool outputValuesUpdated = true;
		public float[] inputValues = new float[ (int) NetworkValue.VALUES_NUMBER ];

		public CompensatorType compensator = new CompensatorType();
	}

	protected Dictionary<KeyValuePair<byte,byte>, NetworkOperator> networkOperators = new Dictionary<KeyValuePair<byte,byte>, NetworkOperator>();

	public override void SetLocalValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType, float value ) 
    {
		KeyValuePair<byte,byte> localKey = new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex );

		if( !networkOperators.ContainsKey( localKey ) ) networkOperators[ localKey ] = new NetworkOperator();

		if( Math.Abs( networkOperators[ localKey ].outputValues[ (int) valueType ] - value ) > 0.1f )
        {
			networkOperators[ localKey ].outputValues[ (int) valueType ] = value;
			networkOperators[ localKey ].outputValuesUpdated = true;

          	//Debug.Log( "Setting " + localKey.ToString() + " position" );
        }
    }

	public override bool HasRemoteKey( int elementID, NetworkAxis axisIndex )
    {
		return networkOperators.ContainsKey( new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex ) );
    }

	public override float GetRemoteValue( int elementID, NetworkAxis axisIndex, NetworkValue valueType )
    {
		NetworkOperator networkOperator;

		if( networkOperators.TryGetValue( new KeyValuePair<byte,byte>( (byte) elementID, (byte) axisIndex ), out networkOperator ) ) 
			return networkOperator.inputValues[ (int) valueType ];

        return 0.0f;
    }

	public override float UpdateData( float updateTime )
	{
		int outputMessageLength = 1;

		if( socketID == -1 ) return updateTime;

		foreach( KeyValuePair<byte,byte> localKey in networkOperators.Keys ) 
		{
			NetworkOperator networkOperator = networkOperators[ localKey ];

			if( networkOperator.outputValuesUpdated )
            {
    			outputBuffer[ outputMessageLength ] = localKey.Key;
    			outputBuffer[ outputMessageLength + 1 ] = localKey.Value;
    			
				networkOperator.compensator.EncodeOutputData( networkOperator.outputValues, outputBuffer, outputMessageLength + 2 );

    			outputMessageLength += DATA_SIZE;

				networkOperator.outputValuesUpdated = false;
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
				if( !networkOperators.ContainsKey( remoteKey ) ) networkOperators[ remoteKey ] = new NetworkOperator();

				networkOperators[ remoteKey ].compensator.DecodeInputData( networkOperators[ remoteKey ].inputValues, inputBuffer, dataOffset + 2 );

	            //Debug.Log( "Received axis " + ( dataOffset / DATA_SIZE ).ToString() + ": " + remoteKey.ToString() + ": " + remoteValues[ remoteKey ].ToString() );
			}
		}
		else
		{
			foreach( NetworkOperator networkOperator in networkOperators.Values )
				networkOperator.compensator.UpdateData( networkOperator.inputValues, networkOperator.outputValues, updateTime );
		}

		return GetNetworkDelay();
	}

	protected abstract void SendUpdateMessage();

	protected abstract bool ReceiveUpdateMessage();
}

