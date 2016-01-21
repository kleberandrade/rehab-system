using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameClient : GameConnection
{
	public override void Connect()
	{
		string gameServerHost = PlayerPrefs.GetString( ConnectionManager.GAME_SERVER_HOST_ID, ConnectionManager.LOCAL_SERVER_HOST );
		ConnectionManager.GameClient.Connect( gameServerHost, 50004 );
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
			}
		}
	}
}

