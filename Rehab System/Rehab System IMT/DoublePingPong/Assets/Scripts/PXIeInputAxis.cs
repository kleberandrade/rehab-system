using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PXIeInputAxis : RemoteInputAxis
{
	const int AXIS_DATA_LENGTH = 2 * sizeof(byte) + 4 * sizeof(float);

	private static byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
	private static byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

	public override void Update( float updateTime )
	{
		bool newDataReceived = ConnectionManager.AxisClient.ReceiveData( inputBuffer );

		outputBuffer[ 0 ] = inputBuffer[ 0 ];

		int axesNumber = (int) inputBuffer[ 0 ];
		int messageLength = sizeof(byte) + axesNumber * AXIS_DATA_LENGTH;

		for( int idPosition = 1; idPosition < messageLength; idPosition += AXIS_DATA_LENGTH ) 
		{
			if( inputBuffer[ idPosition ] == id ) 
			{
				int maskPosition = idPosition + sizeof(byte);
				int dataPosition = idPosition + 2 * sizeof(byte);

				position = BitConverter.ToSingle( inputBuffer, dataPosition ); 
				velocity = BitConverter.ToSingle( inputBuffer, dataPosition + sizeof(float) ); 
				force = BitConverter.ToSingle( inputBuffer, dataPosition + 2 * sizeof(float) ); 

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Received data: p:{0} - v:{1} - f:{2}", position, velocity, force ) );

				outputBuffer[ idPosition ] = id;

				setpointsMask.CopyTo( outputBuffer, maskPosition );
				setpointsMask.SetAll( false );

				Buffer.BlockCopy( BitConverter.GetBytes( feedbackPosition ), 0, outputBuffer, dataPosition, sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( feedbackVelocity ), 0, outputBuffer, dataPosition + sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( stiffness ), 0, outputBuffer, dataPosition + 2 * sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( damping ), 0, outputBuffer, dataPosition + 3 * sizeof(float), sizeof(float) );

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Sending feedback: p:{0} - v:{1} - f:{2} - d:{3}", BitConverter.ToSingle( outputBuffer, dataPosition ), 
					                                                                                       BitConverter.ToSingle( outputBuffer, dataPosition + sizeof(float) ), 
																										   BitConverter.ToSingle( outputBuffer, dataPosition + 2 * sizeof(float) ), 
					                                                                                       BitConverter.ToSingle( outputBuffer, dataPosition + 3 * sizeof(float) ) ) );

				break;
			}
		}

		if( newDataReceived ) ConnectionManager.AxisClient.SendData( outputBuffer );
	}

	public override void Connect()
	{
		string axisServerHost = PlayerPrefs.GetString( ConnectionManager.AXIS_SERVER_HOST_ID, /*ConnectionManager.LOCAL_SERVER_HOST*/"192.168.0.181" );
        ConnectionManager.InfoClient.Connect( axisServerHost, 50000 );
		ConnectionManager.AxisClient.Connect( axisServerHost, 50001 );
	}
}

