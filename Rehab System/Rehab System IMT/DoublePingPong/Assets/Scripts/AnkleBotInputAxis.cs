using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AnkleBotInputAxis : RemoteInputAxis
{
	// Indice relativo da variavel
	public const int CENTERSPRING = 0; 	// Game
	public const int FREESPACE = 1; 	// Game
	public const int STIFF = 2;			// Game
	public const int DAMP = 3;			// Game

	public const int POSITION = 0; 		// Robo
	public const int VELOCITY = 1; 		// Robo
	public const int ACC = 2;			// Robo
	public const int FORCE = 3;			// Robo

	public const int N_VAR = 4; 		// Numero de variaveis envolvidas
	private const int BIT_SIZE = 4; 	// Numero de bit da mascara; Deve ser multiplo de 2
	private const int INFO_SIZE = 4;	// 4 Float; 8 Double

	private const int N_ROBOTS = 2;

	private float delayCount;

	const int DATA_LENGTH = sizeof(byte) + 4 * sizeof(float);

	private static NetworkClientUDP connection = null;

	private static byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
	private static byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

	public AnkleBotInputAxis()
	{
        //setpointsMask.Length = N_ROBOTS * N_VAR;
		setpointsMask = new BitArray( N_ROBOTS * N_VAR, false );
	}

	public override void Update( float updateTime )
	{
		SendMsg();
		ReadMsg();
	}

	public override void Connect( string hostName )
	{
		Debug.Log( "Starting connection" );
		if( connection == null ) connection = new NetworkClientUDP( new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp ) );
		connection.Connect( "192.168.0.66", 8000 );
	}

	private void SendMsg()
	{
		setpointsMask.CopyTo( outputBuffer, 0 );
		setpointsMask.SetAll( false );

		System.Buffer.BlockCopy( BitConverter.GetBytes( feedbackPosition ), 0, outputBuffer, 1 + INFO_SIZE * ( N_VAR * id + CENTERSPRING ), sizeof(float) );
		System.Buffer.BlockCopy( BitConverter.GetBytes( feedbackVelocity ), 0, outputBuffer, 1 + INFO_SIZE * ( N_VAR * id + FREESPACE ), sizeof(float) );
		System.Buffer.BlockCopy( BitConverter.GetBytes( stiffness ), 0, outputBuffer, 1 + INFO_SIZE * ( N_VAR * id + STIFF ), sizeof(float) );
		System.Buffer.BlockCopy( BitConverter.GetBytes( damping ), 0, outputBuffer, 1 + INFO_SIZE * ( N_VAR * id + DAMP ), sizeof(float) );
		
		connection.SendData( outputBuffer );

		return;
	}

	private void ReadMsg()
	{
		connection.ReceiveData( inputBuffer );

		// Check if message is different than zero
		foreach( byte element in inputBuffer )
		{
			if( element != 0x00 )
			{
				position = BitConverter.ToSingle( inputBuffer, 1 + INFO_SIZE * ( N_VAR * id + POSITION ) );
				velocity = BitConverter.ToSingle( inputBuffer, 1 + INFO_SIZE * ( N_VAR * id + VELOCITY ) );
				force = BitConverter.ToSingle( inputBuffer, 1 + INFO_SIZE * ( N_VAR * id + FORCE ) );

				Debug.Log( "Robot " + id.ToString() + ", Pos: " + position.ToString() + ", Vel:" + velocity.ToString() + ", Force:" + force.ToString() );

				break;
			}
		}

		return;
	}

	public override void Disconnect()
	{
		connection.Disconnect();
	}

	~AnkleBotInputAxis()
	{
        Disconnect();
	}
}

