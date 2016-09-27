﻿using UnityEngine;
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
		socketID = NetworkTransport.AddHost( networkTopology );

		string gameServerHost = PlayerPrefs.GetString( GAME_SERVER_HOST_ID, "127.0.0.1" );
		Debug.Log( "Connectingo to host " + gameServerHost + " on port " + GAME_SERVER_PORT );
		connectionID = NetworkTransport.Connect( socketID, gameServerHost, GAME_SERVER_PORT, 0, out connectionError );
		Debug.Log( string.Format( "Added host {0} and connection {1} with channels {2} and {3}", socketID, connectionID, eventChannel, dataChannel )  );
    }

	protected override void SendUpdateMessage()
	{
		//Debug.Log( string.Format( "Sending message from host {0} to connection {1} and client {2}", socketID, connectionID, dataChannel ) );
		NetworkTransport.Send( socketID, connectionID, dataChannel, outputBuffer, PACKET_SIZE, out connectionError );
	}

	protected override bool ReceiveUpdateMessage()
	{
		int remoteConnectionID, channel, receivedSize;
		if( NetworkTransport.ReceiveFromHost( socketID, out remoteConnectionID, out channel, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.DataEvent )
		{
			if( connectionError == (byte) NetworkError.Ok ) 
			{
				//Debug.Log( string.Format( "Received message from connection {0} and channel {1}", connectionID, channel ) );
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

