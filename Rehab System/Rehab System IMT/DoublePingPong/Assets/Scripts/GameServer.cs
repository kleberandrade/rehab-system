﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameServer : GameConnection
{
	private List<int> clientConnections = new List<int>();

	public override void Connect()
    {
		HostTopology networkTopology = new HostTopology( connectionConfig, 10 );
		socketID = NetworkTransport.AddHost( networkTopology, GAME_SERVER_PORT );

		Debug.Log( string.Format( "Added host {0} with channels {1} and {2}", socketID, eventChannel, dataChannel )  );
    }

	protected override void SendUpdateMessage()
	{
		//Debug.Log( "Sending multicast message to channel " + dataChannel.ToString() );
		foreach( int connectionID in clientConnections )
			NetworkTransport.Send( socketID, connectionID, dataChannel, outputBuffer, PACKET_SIZE, out connectionError );
		
		//NetworkTransport.StartSendMulticast( socketID, dataChannel, outputBuffer, PACKET_SIZE, out connectionError );
		//foreach( int connectionID in clientConnections )
		//	NetworkTransport.SendMulticast( socketID, connectionID, out connectionError );
		//NetworkTransport.FinishSendMulticast( socketID, out connectionError );
	}

	protected override bool ReceiveUpdateMessage()
	{
		int connectionID, channel, receivedSize;
		NetworkEventType networkEvent = NetworkTransport.ReceiveFromHost( socketID, out connectionID, out channel, inputBuffer, PACKET_SIZE, out receivedSize, out connectionError );
		if( connectionError == (byte) NetworkError.Ok ) 
		{
			Debug.Log( string.Format( "Received message of type {0} from connection {1} and client {2}", networkEvent, connectionID, channel ) );
			if( networkEvent == NetworkEventType.ConnectEvent ) 
			{
				inputBuffer[ 0 ] = (byte) clientConnections.Count;
				NetworkTransport.Send( socketID, connectionID, eventChannel, inputBuffer, 1, out connectionError );
				clientConnections.Add( connectionID ); 
			}
		    else if( networkEvent == NetworkEventType.DataEvent ) return true;
		}

		return false;
	}

	protected override float GetNetworkDelay()
	{
		return Time.fixedDeltaTime;
	}

	public int GetClientsNumber()
	{
		return clientConnections.Count;
	}
}

