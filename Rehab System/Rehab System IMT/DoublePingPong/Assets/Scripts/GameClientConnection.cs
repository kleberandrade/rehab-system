using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;

public struct ConnectionInfo
{
	public int socketID, connectionID, channel;
	public float sendRate, receiveRate;
	public int rtt, ioTime, lostPackets;
}

public class GameClientConnection : GameConnection
{
	private int connectionID;
	private int clientID = -1;
	private float clientTime = 0.0f;

	private int networkDelayMS = 0;

	public override void Connect()
    {
		HostTopology networkTopology = new HostTopology( connectionConfig, 1 );
		socketID = NetworkTransport.AddHost( networkTopology );

		string gameServerHost = PlayerPrefs.GetString( GAME_SERVER_HOST_ID, Configuration.DEFAULT_IP_HOST );
		Debug.Log( "Connectingo to host " + gameServerHost + " on port " + GAME_SERVER_PORT.ToString() );
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
				if( channel == eventChannel ) 
				{
					clientID = (int) inputBuffer[ 0 ];
					clientTime = BitConverter.ToSingle( inputBuffer, 1 );
				}
				else if( channel == dataChannel ) 
				{
					networkDelay = NetworkTransport.GetCurrentRtt( socketID, connectionID, out connectionError ) / 2000.0f;

					return true;
				}
			}
		}

		return false;
	}

	public int GetClientID()
	{
		return clientID;
	}

	public float GetClientTime()
	{
		return clientTime - networkDelay;
	}

	public ConnectionInfo GetConnectionInfo()
	{
		ConnectionInfo currentConnectionInfo = new ConnectionInfo();
		currentConnectionInfo.socketID = socketID;
		currentConnectionInfo.connectionID = connectionID;
		currentConnectionInfo.channel = dataChannel;
		currentConnectionInfo.sendRate = NetworkTransport.GetPacketSentRate( socketID, connectionID, out connectionError );
		currentConnectionInfo.receiveRate = NetworkTransport.GetPacketReceivedRate( socketID, connectionID, out connectionError );
		currentConnectionInfo.rtt = NetworkTransport.GetCurrentRtt( socketID, connectionID, out connectionError );
		currentConnectionInfo.lostPackets = NetworkTransport.GetNetworkLostPacketNum( socketID, connectionID, out connectionError );

		return currentConnectionInfo;
	}
}

