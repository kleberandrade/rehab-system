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
	private byte[] lastMessage = new byte[ BUFFER_SIZE ];
    private bool hasNewMessage = false;

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
					//Debug.Log( "NetworkClientUDP: Messages available" );

					lock( searchLock )
					{
						try
						{
                            int bytesRead = workSocket.Receive( lastMessage );

							Debug.Log( "Received " + bytesRead.ToString() + " bytes from : " + workSocket.RemoteEndPoint.ToString() );

                            hasNewMessage = true;
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
		try
		{
			lock( searchLock )
			{
                bool isNewMessage = hasNewMessage;

                Buffer.BlockCopy( lastMessage, 0, inputBuffer, 0, Math.Min( inputBuffer.Length, BUFFER_SIZE ) );

                hasNewMessage = false;

                return isNewMessage;
			}
		}
		catch( Exception e ) 
		{
			Debug.Log( e.ToString() );
		}

		return false;
	}

	public override void Disconnect()
	{
		isReceiving = false;
        if( updateThread != null )
        {
            if( updateThread.IsAlive )
            {
                if( !updateThread.Join( 500 ) ) updateThread.Abort();
            }
        }

		base.Disconnect();

		Debug.Log( "Encerrando conexao UDP" );
	}

	~NetworkClientUDP() 
	{
		Disconnect();
	}
}
