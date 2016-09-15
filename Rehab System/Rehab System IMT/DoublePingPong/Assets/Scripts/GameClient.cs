using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameClient : GameConnection
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	private int connectionID, clientID;

	protected override void Connect()
    {
		ConnectionConfig connectionConfig = new ConnectionConfig();
		clientID = connectionConfig.AddChannel( QosType.StateUpdate  ); // QosType.Unreliable sending just most recent

		HostTopology networkTopology = new HostTopology( connectionConfig, 1 );
		socketID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );

		string gameServerHost = PlayerPrefs.GetString( GAME_SERVER_HOST_ID, "127.0.0.1" );
		connectionID = NetworkTransport.Connect( socketID, gameServerHost, GAME_SERVER_PORT, 0, out connectionError );
    }

	protected override void SendUpdateMessage()
	{
		Debug.Log( string.Format( "Sending message from host {0} to connection {1} and client {2}", socketID, connectionID, clientID ) );
		NetworkTransport.Send( socketID, connectionID, clientID, outputBuffer, PACKET_SIZE, out connectionError );
	}

	/*protected override bool ReceiveUpdateMessage()
	{
		int remoteConnectionID, serverID, receivedSize;
		if( NetworkTransport.ReceiveFromHost( broadcastHostID, out remoteConnectionID, out serverID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.BroadcastEvent )
		{
			Debug.Log( "Received broadcast message on connection " + remoteConnectionID.ToString() + " and channel " + serverID.ToString() );
			if( connectionError == (byte) NetworkError.Ok )
			{
				NetworkTransport.GetBroadcastConnectionMessage( broadcastHostID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError );
				return true;
			}
		}

		return false;
	}*/

	protected override bool ReceiveUpdateMessage()
	{
		int remoteConnectionID, serverID, receivedSize;
		if( NetworkTransport.ReceiveFromHost( socketID, out remoteConnectionID, out serverID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError ) == NetworkEventType.DataEvent )
		{
			if( connectionError == (byte) NetworkError.Ok )  return true;
		}

		return false;
	}
}

