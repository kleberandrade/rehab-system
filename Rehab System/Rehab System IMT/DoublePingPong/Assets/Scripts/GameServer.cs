using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameServer : GameConnection
{
	private byte connectedClients = 0;

	protected override void Connect( ConnectionConfig connectionConfig )
    {
		HostTopology networkTopology = new HostTopology( connectionConfig, 10 );
		socketID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );
    }

	protected override void SendUpdateMessage()
	{
		NetworkTransport.StartSendMulticast( socketID, dataChannel, outputBuffer, PACKET_SIZE, out connectionError );
		NetworkTransport.FinishSendMulticast( socketID, out connectionError );
	}

	protected override bool ReceiveUpdateMessage()
	{
		int connectionID, channel, receivedSize;
		NetworkEventType networkEvent = NetworkTransport.ReceiveFromHost( socketID, out connectionID, out channel, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError );
		if( connectionError == (byte) NetworkError.Ok ) 
		{
			Debug.Log( string.Format( "Received message from connection {0} and client {1}", connectionID, channel ) );
			if( networkEvent == NetworkEventType.ConnectEvent ) 
			{
				if( channel == eventChannel ) 
				{
					inputBuffer[ 0 ] = connectedClients++;
					NetworkTransport.Send( socketID, connectionID, eventChannel, inputBuffer, 1, out connectionError );
				}
				else if( channel == dataChannel ) 
					NetworkTransport.SendMulticast( socketID, connectionID, out connectionError );
			}
		    else if( networkEvent == NetworkEventType.DataEvent ) return true;
		}

		return false;
	}

	public int GetClientsNumber()
	{
		return remoteValues.Count;
	}
}

