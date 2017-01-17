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

	protected class InputAxisValue
	{
		public float current = 0.0f, setpoint = 0.0f;
		public float max = 0.0f, min = 0.0f, range = 1.0f;
		public float offset = 0.0f;
	}

	protected InputAxisValue[] inputValues = new InputAxisValue[ Enum.GetValues(typeof(AxisVariable)).Length ];
    protected BitArray setpointsMask = new BitArray( 8, false );

	public virtual bool Init( string axisID )
	{
		id = axisID;

		for( int valueIndex = 0; valueIndex < inputValues.Length; valueIndex++ )
			inputValues[ valueIndex ] = new InputAxisValue();

		return true;
	}

    public virtual void End() {}

	public void Reset()
	{
		foreach( InputAxisValue value in inputValues )
		{
			value.max = value.min = 0.0f;
			value.range = 1.0f;
			value.offset = 0.0f;
		}
	}

	public virtual void Update( float updateTime ) {}

	public float GetValue( AxisVariable variable ) { return inputValues[ (int) variable ].current - inputValues[ (int) variable ].offset; }
	public void SetValue( AxisVariable variable, float value ) 
	{ 
		inputValues[ (int) variable ].setpoint = value + inputValues[ (int) variable ].offset; 
		setpointsMask[ (int) variable ] = true; 
	}

	public float GetNormalizedValue( AxisVariable variable ) 
	{
		InputAxisValue value = inputValues[ (int) variable ];
		return ( 2.0f * ( value.current - value.offset - value.min ) / value.range - 1.0f ); 
	}
	public void SetNormalizedValue( AxisVariable variable, float normalizedValue ) 
	{ 
		InputAxisValue value = inputValues[ (int) variable ];
		value.setpoint = ( ( normalizedValue + 1.0f ) * value.range / 2.0f ) + value.offset + value.min; 
		setpointsMask[ (int) variable ] = true;
	}

	public float GetMinValue( AxisVariable variable ) {	return inputValues[ (int) variable ].min; }
	public float GetMaxValue( AxisVariable variable ) {	return inputValues[ (int) variable ].max; }

	public void SetMinValue( AxisVariable variable, float value ) 
	{ 
		inputValues[ (int) variable ].min = value;
		Calibrate( variable );
	}
	public void SetMaxValue( AxisVariable variable, float value ) 
	{ 
		inputValues[ (int) variable ].max = value;
		Calibrate( variable );
	}

	private void Calibrate( AxisVariable variable )
	{
		InputAxisValue value = inputValues[ (int) variable ];
		value.range = value.max - value.min;
		if( Mathf.Approximately( value.range, 0.0f ) ) value.range = 1.0f;
	}
		
	public void SetOffset() 
	{ 
		for( int valueIndex = 0; valueIndex < inputValues.Length; valueIndex++ )
			inputValues[ valueIndex ].offset = inputValues[ valueIndex ].current; 
	}
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
		inputValues[ (int) AxisVariable.VELOCITY ].current = Input.GetAxis( id ) / updateTime;
		inputValues[ (int) AxisVariable.POSITION ].current += inputValues[ (int) AxisVariable.VELOCITY ].current * updateTime;
		inputValues[ (int) AxisVariable.FORCE ].current = inputValues[ (int) AxisVariable.VELOCITY ].current;
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
		//if( ! Mathf.Approximately( feedbackPosition, position ) ) position = feedbackPosition;
		inputValues[ (int) AxisVariable.VELOCITY ].current = Input.GetAxis( id );
		inputValues[ (int) AxisVariable.POSITION ].current += inputValues[ (int) AxisVariable.VELOCITY ].current * updateTime;
		//feedbackPosition = position;
		inputValues[ (int) AxisVariable.FORCE ].current = inputValues[ (int) AxisVariable.VELOCITY ].current;
	}
}



public class RemoteInputAxis : InputAxis
{
	private class AxisConnection
	{
		public string hostID;
		public InputAxisDataClient dataClient = null;
		public byte[] inputBuffer = new byte[ InputAxisClient.BUFFER_SIZE ];
		public byte[] outputBuffer = new byte[ InputAxisClient.BUFFER_SIZE ];
		public bool outputUpdated = false;

		public AxisConnection( string hostID )
		{
			this.hostID = hostID;
			dataClient = new InputAxisDataClient();
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

				for( int valueIndex = 0; valueIndex < inputValues.Length; valueIndex++ )
					inputValues[ valueIndex ].current = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + valueIndex * sizeof(float) );

				int outputIDPosition = 1 + axisIndex * OUTPUT_DATA_LENGTH;
				int outputMaskPosition = outputIDPosition + sizeof(byte);
				int outputDataPosition = inputIDPosition + 2 * sizeof(byte);

				axis.outputBuffer[ outputIDPosition ] = index;

				setpointsMask.CopyTo( axis.outputBuffer, outputMaskPosition );
				if( axis.outputBuffer[ outputMaskPosition ] > 0 ) axis.outputUpdated = true;
				setpointsMask.SetAll( false );

				//for( int valueIndex = 0; valueIndex < inputValues.Length; valueIndex++ )
				//	if( ! Mathf.Approximately( inputValues[ valueIndex ].setpoint, inputValues[ valueIndex ].value ) ) axis.outputUpdated = true;

				if( axis.outputUpdated )
				{
					for( int valueIndex = 0; valueIndex < inputValues.Length; valueIndex++ )
						Buffer.BlockCopy( BitConverter.GetBytes( inputValues[ valueIndex ].setpoint ), 0, axis.outputBuffer, outputDataPosition + valueIndex * sizeof(float), sizeof(float) );
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

