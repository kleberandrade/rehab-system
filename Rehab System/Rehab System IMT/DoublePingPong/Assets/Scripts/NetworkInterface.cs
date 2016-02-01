using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public abstract class NetworkInterface
{
	protected Socket workSocket = null;

	public const int BUFFER_SIZE = 256;
}

public abstract class NetworkClient : NetworkInterface
{
	private string currentHost = "localhost";
	private int currentRemotePort = 0;

	public NetworkClient( Socket newSocket = null )
	{
		workSocket = newSocket;
	}

	private void connectCallback( IAsyncResult ar ) 
	{		
		try 
		{
			Socket handle = (Socket) ar.AsyncState;
			
			handle.EndConnect( ar );

			Debug.Log( "Bound to: " + workSocket.LocalEndPoint.ToString() );
			Debug.Log( "Connected to: " + workSocket.RemoteEndPoint.ToString() );
		}
		catch( Exception e ) 
		{
			Debug.Log( e.ToString() );
		}
	}

	private void writeCallback( IAsyncResult ar ) 
	{
		try 
		{
			Socket handle = (Socket) ar.AsyncState;
			
			int bytesSent = handle.EndSend( ar );
			
			Debug.Log( "Sent " + bytesSent.ToString() + " bytes to: " + handle.RemoteEndPoint.ToString() );
		}
		catch( Exception e ) 
		{		
			Debug.Log( e.ToString() );
		}
	}

	public virtual void Connect( string host, int remotePort ) 
	{
		if( !workSocket.Connected || host != currentHost || remotePort != currentRemotePort ) 
		{
			Debug.Log( "Trying to connect to host " + host + " and port " + remotePort.ToString() );
			try 
			{
				Debug.Log( "Connecting to: host: " + host + " - port: " + remotePort.ToString() );

				workSocket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );
				workSocket.ExclusiveAddressUse = false;
				workSocket.ReceiveBufferSize = BUFFER_SIZE;
				workSocket.SendBufferSize = BUFFER_SIZE;

				IPAddress ipRemoteHost = Dns.GetHostEntry( host ).AddressList[ 0 ];
				Debug.Log( ipRemoteHost.ToString() );
				IPEndPoint remoteIpAddress = new IPEndPoint( ipRemoteHost, remotePort );
				workSocket.BeginConnect( (EndPoint) remoteIpAddress, connectCallback, workSocket );

				currentHost = host;
				currentRemotePort = remotePort;
			} 
			catch( Exception e ) 
			{
				Disconnect();
				Debug.Log( e.ToString() );
			}
		}
	}

	public abstract bool ReceiveData( byte[] inputBuffer );

	public void SendData( byte[] outputBuffer ) 
	{
		if( workSocket.Connected ) 
		{
			try 
			{	
				workSocket.BeginSend( outputBuffer, 0, outputBuffer.Length, SocketFlags.None, new AsyncCallback( writeCallback ), workSocket );
			} 
			catch( Exception e ) 
			{
				Debug.Log( e.ToString() );
			}	
		}
	}

	public virtual void Disconnect()
	{
		try
		{
			workSocket.Close();
		}
		catch( Exception e )
		{
			Debug.Log( e.ToString() );
		}

		currentHost = "localhost";
		currentRemotePort = 0;

		Debug.Log( "Encerrando conexao" );
	}

	~NetworkClient() 
	{
		Disconnect();
	}
}

public abstract class NetworkServer : NetworkInterface
{
	protected List<Socket> clientSockets = new List<Socket>();

	Thread listeningThread;
	bool isListening;

	int currentLocalPort = 0;

	public void StartListening( int localPort )
	{
		if( !isListening || localPort != currentLocalPort ) 
		{
			workSocket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );
			workSocket.ExclusiveAddressUse = false;

			IPEndPoint localIpAddress = new IPEndPoint( IPAddress.Any, localPort );
			workSocket.Bind( (EndPoint) localIpAddress );

			workSocket.Listen( 10 );

			if (!isListening) 
			{
				listeningThread = new Thread( new ThreadStart( ListeningCallBack ) );
				listeningThread.Start ();
			}
		}
	}

	public void StopListening()
	{
		isListening = false;
		if( listeningThread != null )
			listeningThread.Join();

		try
		{
			workSocket.Close();
		}
		catch( Exception e )
		{
			Debug.Log( e.ToString() );
		}

		currentLocalPort = 0;
	}

	public void ListeningCallBack()
	{
		isListening = true;

		Debug.Log( "NetworkServer: Starting to receive messages" );

		try 
		{
			while( isListening )
			{
				if( workSocket.Available > 0 )
				{
					Debug.Log( "NetworkClientUDP: Messages available" );

					try
					{
						Socket clientSocket = workSocket.Accept();

						Debug.Log( "Accepted client from : " + clientSocket.RemoteEndPoint.ToString() );

						clientSockets.Add( clientSocket );
					}
					catch( SocketException e )
					{
						Debug.Log( e.ToString() );
					}
				} 
			}
		}
		catch( ObjectDisposedException e ) 
		{
			Debug.Log( e.ToString() );
		}

		StopListening();

		Debug.Log( "NetworkServer: Finishing update thread" );
	}

	public abstract NetworkClient AcceptClient();

	~NetworkServer()
	{
		StopListening();
	}
}