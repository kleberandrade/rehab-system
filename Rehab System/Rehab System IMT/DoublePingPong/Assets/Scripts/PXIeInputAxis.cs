using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PXIeInputAxis : RemoteInputAxis
{
	private class AxisConnection
	{
		public string hostName;
		public NetworkClientUDP dataClient = null;
		public byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
		public byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

		public AxisConnection( string hostName )
		{
			this.hostName = hostName;
			dataClient = new NetworkClientUDP();
			dataClient.Connect( hostName, 50001 );
		}
	}

	const int AXIS_DATA_LENGTH = 2 * sizeof(byte) + 4 * sizeof(float);

	private static List<AxisConnection> axisConnections = new List<AxisConnection>();

	private AxisConnection axis;

	public override void Connect( string hostName )
    {
		axis = axisConnections.Find( connection => connection.hostName == hostName );
		if( axis == null ) axisConnections.Add( new AxisConnection( hostName ) );
    }

	public override void Update( float updateTime )
	{
		bool newDataReceived = axis.dataClient.ReceiveData( axis.inputBuffer );

		axis.outputBuffer[ 0 ] = axis.inputBuffer[ 0 ];

		int axesNumber = (int) axis.inputBuffer[ 0 ];
		int messageLength = sizeof(byte) + axesNumber * AXIS_DATA_LENGTH;

		for( int idPosition = 1; idPosition < messageLength; idPosition += AXIS_DATA_LENGTH ) 
		{
			if( axis.inputBuffer[ idPosition ] == id ) 
			{
				int maskPosition = idPosition + sizeof(byte);
				int dataPosition = idPosition + 2 * sizeof(byte);

				position = BitConverter.ToSingle( axis.inputBuffer, dataPosition ); 
				velocity = BitConverter.ToSingle( axis.inputBuffer, dataPosition + sizeof(float) ); 
				force = BitConverter.ToSingle( axis.inputBuffer, dataPosition + 2 * sizeof(float) ); 

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Received data: p:{0} - v:{1} - f:{2}", position, velocity, force ) );

				axis.outputBuffer[ idPosition ] = id;

				setpointsMask.CopyTo( axis.outputBuffer, maskPosition );
				setpointsMask.SetAll( false );

				Buffer.BlockCopy( BitConverter.GetBytes( feedbackPosition ), 0, axis.outputBuffer, dataPosition, sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( feedbackVelocity ), 0, axis.outputBuffer, dataPosition + sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( stiffness ), 0, axis.outputBuffer, dataPosition + 2 * sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( damping ), 0, axis.outputBuffer, dataPosition + 3 * sizeof(float), sizeof(float) );

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Sending feedback: p:{0} - v:{1} - f:{2} - d:{3}", BitConverter.ToSingle( axis.outputBuffer, dataPosition ), 
					                                                                                       BitConverter.ToSingle( axis.outputBuffer, dataPosition + sizeof(float) ), 
																										   BitConverter.ToSingle( axis.outputBuffer, dataPosition + 2 * sizeof(float) ), 
					                                                                                       BitConverter.ToSingle( axis.outputBuffer, dataPosition + 3 * sizeof(float) ) ) );

				break;
			}
		}

		if( newDataReceived ) axis.dataClient.SendData( axis.outputBuffer );
	}
       
    public override void Disconnect()
    {
        //ConnectionManager.InfoClient.Disconnect();
		axis.dataClient.Disconnect();
    }
}

