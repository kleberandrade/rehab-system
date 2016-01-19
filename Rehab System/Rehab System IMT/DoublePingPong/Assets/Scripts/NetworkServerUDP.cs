using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class NetworkServerUDP : NetworkServer
{
	public NetworkServerUDP()
	{
		try 
		{
			workSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
		}
		catch( Exception e ) 
		{
			Debug.Log( e.ToString() );
		}
	}

	public override NetworkClient AcceptClient()
	{
		if( clientSockets.Count == 0 ) return null;

		Socket clientSocket = clientSockets[ 0 ];
		clientSockets.RemoveAt( 0 );

		return new NetworkClientUDP( clientSocket );
	}
}