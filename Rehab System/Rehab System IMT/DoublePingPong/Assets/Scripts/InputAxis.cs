using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InputAxis
{
	protected string name;
	public string Name { get { return name; } }

	protected float position = 0.0f, velocity = 0.0f, force = 0.0f;
	private float positionOffset = 0.0f, forceOffset = 0.0f;
	private float maxValue = 0.0f, minValue = 0.0f, range = 1.0f;

	protected float feedbackPosition = 0.0f, feedbackVelocity = 0.0f;
	protected float stiffness = 0.0f, damping = 0.0f;

    protected BitArray setpointsMask = new BitArray( 4, false );

	public virtual bool Init( string axisName )
	{
		name = axisName;

		return true;
	}

    public virtual void End() {}

	public void Reset()
	{
		positionOffset = 0.0f;
		forceOffset = 0.0f;
		maxValue = 0.0f;
		minValue = 0.0f;
		range = 1.0f;
	}

	public virtual void Update( float updateTime ) {}

	public float Position { get { return position - positionOffset; } set { feedbackPosition = value + positionOffset; setpointsMask[ 0 ] = true; } }
	public float Velocity { get { return velocity; } set { feedbackVelocity = value; setpointsMask[ 1 ] = true; } }
	public float Force { get { return force - forceOffset; } }

	public float PositionOffset { set { positionOffset = value; } }
	public float ForceOffset { set { forceOffset = value; } }

	public float Stiffness { set { stiffness = value; setpointsMask[ 2 ] = true; } }
	public float Damping { set { damping = value; setpointsMask[ 3 ] = true; } }

	public float NormalizedPosition { get { return ( 2 * ( position - minValue ) / range - 1.0f ); } 
		                              set { feedbackPosition = ( ( value + 1.0f ) * range / 2.0f ) + minValue; setpointsMask[ 0 ] = true; } }
	public float NormalizedVelocity { get { return ( 2 * velocity / range ); } 
		                              set { feedbackVelocity = ( value * range / 2.0f ); setpointsMask[ 1 ] = true; } }
	public float NormalizedForce { get { return ( 2 * ( force - minValue ) / range - 1.0f ); } }

    public float MaxValue { get { return maxValue; } set { maxValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
    public float MinValue { get { return minValue; } set { minValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
}

public class LocalInputAxis : InputAxis
{
	public override void Update( float updateTime )
	{
		position += velocity * updateTime;
	}
}

public class RemoteInputAxis : InputAxis
{
	private class AxisConnection
	{
		public string hostName;
		public NetworkClientUDP dataClient = null;
		public byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
		public byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

		public AxisConnection( string hostName )
		{
			this.hostName = hostName;
			dataClient = new NetworkClientUDP();
			dataClient.Connect( hostName, 50001 );
		}
	}

	public const string AXIS_SERVER_HOST = "AXIS_SERVER_HOST";

	const int AXIS_DATA_LENGTH = 7 * sizeof(float);
	const int INPUT_DATA_LENGTH = 2 * sizeof(byte) + AXIS_DATA_LENGTH;
	const int OUTPUT_DATA_LENGTH = sizeof(byte) + AXIS_DATA_LENGTH;

	private static List<AxisConnection> axisConnections = new List<AxisConnection>();

	private byte id;
	private AxisConnection axis;

	public override bool Init( string axisName )
	{
		string axisHost = PlayerPrefs.GetString( AXIS_SERVER_HOST, "192.168.0.66" );

		base.Init( axisName );

        if( byte.TryParse( axisName, out id ) )
        {
			axis = axisConnections.Find( connection => connection.hostName == axisHost );
			if( axis == null ) 
			{
				axis = new AxisConnection( axisHost );
				axisConnections.Add( axis );
			}
            return true;
        }

        return false;
	}

    public override void End()
    {
		//ConnectionManager.InfoClient.Disconnect();
		axis.dataClient.Disconnect();
    }

	public override void Update( float updateTime )
	{
		bool newDataReceived = axis.dataClient.ReceiveData( axis.inputBuffer );

		axis.outputBuffer[ 0 ] = axis.inputBuffer[ 0 ];

		int axesNumber = (int) axis.inputBuffer[ 0 ];
		for( int axisIndex = 0; axisIndex < axesNumber; axisIndex++ ) 
		{
			int inputIDPosition = 1 + axisIndex * INPUT_DATA_LENGTH;

			if( axis.inputBuffer[ inputIDPosition ] == id ) 
			{
				int inputDataPosition = inputIDPosition + sizeof(byte);

				position = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition ); 
				velocity = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + sizeof(float) ); 
				force = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + 3 * sizeof(float) ); 

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Received data: p:{0} - v:{1} - f:{2}", position, velocity, force ) );

				int outputIDPosition = 1 + axisIndex * OUTPUT_DATA_LENGTH;
				int outputMaskPosition = outputIDPosition + sizeof(byte);
				int outputDataPosition = inputIDPosition + 2 * sizeof(byte);

				axis.outputBuffer[ outputIDPosition ] = id;

				setpointsMask.CopyTo( axis.outputBuffer, outputMaskPosition );
				setpointsMask.SetAll( false );

				Buffer.BlockCopy( BitConverter.GetBytes( feedbackPosition ), 0, axis.outputBuffer, outputDataPosition, sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( feedbackVelocity ), 0, axis.outputBuffer, outputDataPosition + sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( stiffness ), 0, axis.outputBuffer, outputDataPosition + 4 * sizeof(float), sizeof(float) );
				Buffer.BlockCopy( BitConverter.GetBytes( damping ), 0, axis.outputBuffer, outputDataPosition + 5 * sizeof(float), sizeof(float) );

				// Debug
				if( id == 0 ) Debug.Log( string.Format( "Sending feedback: p:{0} - v:{1} - f:{2} - d:{3}", BitConverter.ToSingle( axis.outputBuffer, outputDataPosition ), 
					BitConverter.ToSingle( axis.outputBuffer, outputDataPosition + sizeof(float) ), 
					BitConverter.ToSingle( axis.outputBuffer, outputDataPosition + 4 * sizeof(float) ), 
					BitConverter.ToSingle( axis.outputBuffer, outputDataPosition + 5 * sizeof(float) ) ) );

				break;
			}
		}

		if( newDataReceived ) axis.dataClient.SendData( axis.outputBuffer );
	}
}

public class MouseInputAxis : LocalInputAxis
{
	public override bool Init( string axisName ) 
	{
		base.Init( axisName );

		if( axisName == "Mouse X" || axisName == "Mouse Y" ) return true;

		return false;
	}

	public override void Update( float updateTime )
	{
		velocity = Input.GetAxis( name ) / updateTime;
		base.Update( updateTime );
	}
}

public class KeyboardInputAxis : LocalInputAxis
{
	public override bool Init( string axisName ) 
	{
		base.Init( axisName );

		if( axisName == "Horizontal" || axisName == "Vertical" ) return true;

		return false;
	}

	public override void Update( float updateTime )
	{
		velocity = Input.GetAxis( name );
		base.Update( updateTime );
	}
}

