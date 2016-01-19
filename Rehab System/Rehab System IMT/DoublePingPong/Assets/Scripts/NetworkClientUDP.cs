using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetworkClientUDP : NetworkClient 
{
	private const int QUEUE_LENGTH = 5;
	private byte[][] messageQueue = new byte[ QUEUE_LENGTH ][] { new byte[ BUFFER_SIZE ], new byte[ BUFFER_SIZE ], new byte[ BUFFER_SIZE ], new byte[ BUFFER_SIZE ], new byte[ BUFFER_SIZE ] };
	private int firstIndex = 0, lastIndex = 0;

	private Thread updateThread = null;
	private bool isReceiving = false;

	private object searchLock = new object();

	public NetworkClientUDP() 
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

	public NetworkClientUDP( Socket clientSocket ) : base( clientSocket ) {	}

	public override void Connect( string host, int remotePort ) 
	{	
		//client.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true );

		base.Connect( host, remotePort );

		if( !isReceiving )
		{
			updateThread = new Thread( new ThreadStart( UpdateCallback ) );
			updateThread.Start();

			byte[] nullBuffer = new byte[ BUFFER_SIZE ];
			SendData( nullBuffer );
		}
	}

	private void UpdateCallback() 
	{	
		isReceiving = true;

		Debug.Log( "NetworkClientUDP: Starting to receive messages" );

		try 
		{
			while( isReceiving )
			{
				if( workSocket.Available > 0 )
				{
					Debug.Log( "NetworkClientUDP: Messages available" );

					lock( searchLock )
					{
						try
						{
							int bytesRead = workSocket.Receive( messageQueue[ lastIndex % QUEUE_LENGTH ] );

							Debug.Log( "Received " + bytesRead.ToString() + " bytes from : " + workSocket.RemoteEndPoint.ToString() );

							lastIndex++;
						}
						catch( SocketException e )
						{
							Debug.Log( e.ToString() );
						}
					}
				} 
			}
		}
		catch( ObjectDisposedException e ) 
		{
			Debug.Log( e.ToString() );
		}
		
		Disconnect();
			
		Debug.Log( "NetworkClientUDP: Finishing update thread" );
	}

	public override bool ReceiveData( byte[] inputBuffer ) 
	{	
		if( lastIndex - firstIndex > 0 )
		{
			try
			{
				lock( searchLock )
				{
					Buffer.BlockCopy( messageQueue[ firstIndex % QUEUE_LENGTH ], 0, inputBuffer, 0, BUFFER_SIZE );

					firstIndex++;

					return true;
				}
			}
			catch( Exception e ) 
			{
				Debug.Log( e.ToString() );
			}
		}

		return false;
	}

	public override void Disconnect()
	{
		isReceiving = false;
		if( updateThread != null )
			updateThread.Join();

		base.Disconnect();

		Debug.Log( "Encerrando conexao UDP" );
	}

	~NetworkClientUDP() 
	{
		Disconnect();
	}
}
