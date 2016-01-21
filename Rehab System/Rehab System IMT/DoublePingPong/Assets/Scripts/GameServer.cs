using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameServer : GameConnection
{
	public override void Connect()
	{
		ConnectionManager.GameServer.StartListening( 50004 );
	}

	void Update()
	{
		NetworkClient newClient = ConnectionManager.GameServer.AcceptClient();
		if( newClient != null ) remoteClients.Add( newClient );

		int outputMessageLength = 1;

		foreach( KeyValuePair<byte,byte> localPositionKey in localPositions.Keys ) 
		{
			outputBuffer[ outputMessageLength ] = localPositionKey.Key;
			outputBuffer[ outputMessageLength + 1 ] = localPositionKey.Value;
			Buffer.BlockCopy( BitConverter.GetBytes( localPositions[ localPositionKey ] ), 0, outputBuffer, outputMessageLength + 2, sizeof(float) );

			outputMessageLength += DATA_SIZE;
		}

		foreach( NetworkClient client in remoteClients )
		{
			if( client.ReceiveData( inputBuffer ) )
			{
				Buffer.BlockCopy( inputBuffer, 0, outputBuffer, outputMessageLength, DATA_SIZE );

				KeyValuePair<byte,byte> remotePositionKey = new KeyValuePair<byte,byte>( inputBuffer[ 0 ], inputBuffer[ 1 ] );
				remotePositions[ remotePositionKey ] = BitConverter.ToSingle( inputBuffer, 2 );
			}

			outputMessageLength += DATA_SIZE;
		}

		outputBuffer[ 0 ] = (byte) outputMessageLength;

		foreach( NetworkClient client in remoteClients )
			client.SendData( outputBuffer );
	}
}

