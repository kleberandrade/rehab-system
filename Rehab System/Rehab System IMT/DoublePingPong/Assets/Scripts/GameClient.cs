using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameClient : GameConnection
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	private int connectionID;
	private int clientID = -1;

	protected override void Connect( ConnectionConfig connectionConfig )
    {
		HostTopology networkTopology = new HostTopology( connectionConfig, 1 );
		socketID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );

		string gameServerHost = PlayerPrefs.GetString( GAME_SERVER_HOST_ID, "127.0.0.1" );
		connectionID = NetworkTransport.Connect( socketID, gameServerHost, GAME_SERVER_PORT, 0, out connectionError );
    }

	protected override void SendUpdateMessage()
	{
		Debug.Log( string.Format( "Sending message from host {0} to connection {1} and client {2}", socketID, connectionID, dataChannel ) );
		NetworkTransport.Send( socketID, connectionID, dataChannel, outputBuffer, PACKET_SIZE, out connectionError );
	}

	protected override bool ReceiveUpdateMessage()
	{
		int remoteConnectionID, channel, receivedSize;
		if( NetworkTransport.ReceiveFromHost( socketID, out remoteConnectionID, out channel, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.DataEvent )
		{
			if( connectionError == (byte) NetworkError.Ok ) 
			{
				if( channel == eventChannel ) clientID = (int) inputBuffer[ 0 ];
				else if( channel == dataChannel ) return true;
			}
		}

		return false;
	}

	public int ReceiveClientID()
	{
		return clientID;
	}
}

