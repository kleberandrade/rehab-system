using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameServer : GameConnection
{
	private int serverID = -1;

	protected override void Connect()
    {
		ConnectionConfig connectionConfig = new ConnectionConfig();
		serverID = connectionConfig.AddChannel( QosType.StateUpdate );

		HostTopology networkTopology = new HostTopology( connectionConfig, 10 );
		socketID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );
    }

	protected override void SendUpdateMessage()
	{
		NetworkTransport.StartSendMulticast( socketID, serverID, outputBuffer, PACKET_SIZE, out connectionError );
		NetworkTransport.FinishSendMulticast( socketID, out connectionError );
	}

	protected override bool ReceiveUpdateMessage()
	{
		int connectionID, clientID, receivedSize;
		NetworkEventType networkEvent = NetworkTransport.ReceiveFromHost( socketID, out connectionID, out clientID, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError );
		if( connectionError == (byte) NetworkError.Ok ) 
		{
			Debug.Log( string.Format( "Received message from connection {0} and client {1}", connectionID, clientID ) );
			if( networkEvent == NetworkEventType.ConnectEvent ) NetworkTransport.SendMulticast( socketID, connectionID, out connectionError );
		    else if( networkEvent == NetworkEventType.DataEvent ) return true;
		}

		return false;
	}
}

