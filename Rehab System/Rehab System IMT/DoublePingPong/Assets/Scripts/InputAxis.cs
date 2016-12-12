using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum AxisVariable { POSITION, VELOCITY, FORCE, ACCELERATION, STIFFNESS, DAMPING };

public class InputAxis
{
	protected string id;
	public string ID { get { return id; } }

	protected float position = 0.0f, velocity = 0.0f, force = 0.0f;
	private float positionOffset = 0.0f, forceOffset = 0.0f;
	private float maxValue = 0.0f, minValue = 0.0f, range = 1.0f;

	protected float feedbackPosition = 0.0f, feedbackVelocity = 0.0f, feedbackForce = 0.0f;
	protected float stiffness = 0.0f, damping = 0.0f;

    protected BitArray setpointsMask = new BitArray( 8, false );

	public virtual bool Init( string axisID )
	{
		id = axisID;

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

	public float Position { get { return position - positionOffset; } set { feedbackPosition = value + positionOffset; setpointsMask[ (int) AxisVariable.POSITION ] = true; } }
	public float Velocity { get { return velocity; } set { feedbackVelocity = value; setpointsMask[ (int) AxisVariable.VELOCITY ] = true; } }
	public float Force { get { return force - forceOffset; } set { feedbackForce = value + forceOffset; setpointsMask[ (int) AxisVariable.FORCE ] = true; } }

	public float PositionOffset { set { positionOffset = value; } }
	public float ForceOffset { set { forceOffset = value; } }

	public float Stiffness { get { return stiffness; } set { stiffness = value; setpointsMask[ (int) AxisVariable.STIFFNESS ] = true; } }
	public float Damping { get { return damping; } set { damping = value; setpointsMask[ (int) AxisVariable.DAMPING ] = true; } }

	public float NormalizedPosition { get { return ( 2 * ( position - minValue ) / range - 1.0f ); } 
									  set { feedbackPosition = ( ( value + 1.0f ) * range / 2.0f ) + minValue; setpointsMask[ (int) AxisVariable.POSITION ] = true; } }
	public float NormalizedVelocity { get { return ( 2 * velocity / range ); } 
									  set { feedbackVelocity = ( value * range / 2.0f ); setpointsMask[ (int) AxisVariable.VELOCITY ] = true; } }
	public float NormalizedForce { get { return ( 2 * ( force - minValue ) / range - 1.0f ); } }

    public float MaxValue { get { return maxValue; } set { maxValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
    public float MinValue { get { return minValue; } set { minValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
}


public class MouseInputAxis : InputAxis
{
	public static readonly List<string> DEFAULT_AXIS_NAMES = new List<string> { "Mouse X", "Mouse Y" };

	public override bool Init( string axisID ) 
	{
		base.Init( axisID );

		if( DEFAULT_AXIS_NAMES.Contains( axisID ) ) return true;

		return false;
	}

	public override void Update( float updateTime )
	{
		velocity = Input.GetAxis( id ) / updateTime;
		position += velocity * updateTime;
	}
}

public class KeyboardInputAxis : InputAxis
{
	public static readonly List<string> DEFAULT_AXIS_NAMES = new List<string> { "Horizontal", "Vertical" };

	public override bool Init( string axisID ) 
	{
		base.Init( axisID );

		if( DEFAULT_AXIS_NAMES.Contains( axisID ) ) return true;

		return false;
	}

	public override void Update( float updateTime )
	{
		if( ! Mathf.Approximately( feedbackPosition, position ) ) position = feedbackPosition;
		velocity = Input.GetAxis( id );
		position += velocity * updateTime;
		feedbackPosition = position;
	}
}



public class RemoteInputAxis : InputAxis
{
	private class AxisConnection
	{
		public string hostID;
		public AxisDataClient dataClient = null;
		public byte[] inputBuffer = new byte[ AxisClient.BUFFER_SIZE ];
		public byte[] outputBuffer = new byte[ AxisClient.BUFFER_SIZE ];
		public bool outputUpdated = false;

		public AxisConnection( string hostID )
		{
			this.hostID = hostID;
			dataClient = new AxisDataClient();
			dataClient.Connect( hostID, 50001 );
		}
	}

	public const string AXIS_SERVER_HOST_ID = "Axis Server Host";

	const int AXIS_DATA_LENGTH = 7 * sizeof(float);
	const int INPUT_DATA_LENGTH = 2 * sizeof(byte) + AXIS_DATA_LENGTH;
	const int OUTPUT_DATA_LENGTH = sizeof(byte) + AXIS_DATA_LENGTH;

	private static List<AxisConnection> axisConnections = new List<AxisConnection>();

	private byte index;
	private AxisConnection axis;

	public override bool Init( string axisID )
	{
		Debug.Log( "Initializing remote input axis with ID " + axisID.ToString() );

		string axisHost = PlayerPrefs.GetString( AXIS_SERVER_HOST_ID, Configuration.DEFAULT_IP_HOST );

		base.Init( axisID );

		if( byte.TryParse( axisID, out index ) )
        {
			axis = axisConnections.Find( connection => connection.hostID == axisHost );
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
		axis.dataClient.Disconnect();
    }

	public override void Update( float updateTime )
	{
		/*bool newDataReceived =*/ axis.dataClient.ReceiveData( axis.inputBuffer );

		axis.outputBuffer[ 0 ] = axis.inputBuffer[ 0 ];

		int axesNumber = (int) axis.inputBuffer[ 0 ];
		for( int axisIndex = 0; axisIndex < axesNumber; axisIndex++ ) 
		{
			int inputIDPosition = 1 + axisIndex * INPUT_DATA_LENGTH;

			if( axis.inputBuffer[ inputIDPosition ] == index ) 
			{
				int inputDataPosition = inputIDPosition + sizeof(byte);

				position = -BitConverter.ToSingle( axis.inputBuffer, inputDataPosition ); 
				velocity = -BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + sizeof(float) ); 
				force = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + 2 * sizeof(float) ); 

				// Debug
				//if( index == 0 ) Debug.Log( string.Format( "Received data: p:{0} - v:{1} - f:{2}", position, velocity, force ) );

				int outputIDPosition = 1 + axisIndex * OUTPUT_DATA_LENGTH;
				int outputMaskPosition = outputIDPosition + sizeof(byte);
				int outputDataPosition = inputIDPosition + 2 * sizeof(byte);

				axis.outputBuffer[ outputIDPosition ] = index;

				setpointsMask.CopyTo( axis.outputBuffer, outputMaskPosition );
				if( axis.outputBuffer[ outputMaskPosition ] > 0 ) axis.outputUpdated = true;
				setpointsMask.SetAll( false );

				//if( ! Mathf.Approximately( feedbackPosition, position ) ) axis.outputUpdated = true;
				//if( ! Mathf.Approximately( feedbackVelocity, velocity ) ) axis.outputUpdated = true;
				//if( ! Mathf.Approximately( feedbackForce, force ) ) axis.outputUpdated = true;
				//if( ! Mathf.Approximately( stiffness, 0.0f ) ) axis.outputUpdated = true;

				if( axis.outputUpdated )
				{
					Buffer.BlockCopy( BitConverter.GetBytes( feedbackPosition ), 0, axis.outputBuffer, outputDataPosition, sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( feedbackVelocity ), 0, axis.outputBuffer, outputDataPosition + sizeof(float), sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( feedbackForce ), 0, axis.outputBuffer, outputDataPosition + 2 * sizeof(float), sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( stiffness ), 0, axis.outputBuffer, outputDataPosition + 4 * sizeof(float), sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( damping ), 0, axis.outputBuffer, outputDataPosition + 5 * sizeof(float), sizeof(float) );
				}

				break;
			}
		}

		if( /*newDataReceived &&*/ axis.outputUpdated ) 
		{
			Debug.Log( "Sending setpoints with mask: " + axis.outputBuffer[ 2 ].ToString() );
			axis.dataClient.SendData( axis.outputBuffer );
			axis.outputUpdated = false;
		}
	}
}

