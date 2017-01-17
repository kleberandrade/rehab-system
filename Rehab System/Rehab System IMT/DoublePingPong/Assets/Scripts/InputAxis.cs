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

	private struct InputVariable
	{
		public float value = 0.0f, setpoint = 0.0f;
		public float max = 0.0f, min = 0.0f, range = 1.0f;
		public float offset = 0.0f;
	}

	protected InputVariable[] inputVariables = new InputVariable[ Enum.GetValues(typeof(AxisVariable)).Length ];
    protected BitArray setpointsMask = new BitArray( 8, false );

	public virtual bool Init( string axisID )
	{
		id = axisID;
		return true;
	}

    public virtual void End() {}

	public void Reset()
	{
		foreach( InputVariable variable in inputVariables )
		{
			variable.max = variable.min = 0.0f;
			variable.range = 1.0f;
			variable.offset = 0.0f;
		}
	}

	public virtual void Update( float updateTime ) {}

	public float GetValue( AxisVariable axis ) { return inputVariables[ (int) axis ].value - inputVariables[ (int) axis ].offset; }
	public void SetValue( AxisVariable axis, float value ) 
	{ 
		inputVariables[ (int) axis ].setpoint = value + inputVariables[ (int) axis ].offset; 
		setpointsMask[ (int) axis ] = true; 
	}

	public float GetNormalizedValue( AxisVariable axis ) 
	{
		InputVariable variable = inputVariables[ (int) axis ];
		return ( 2.0f * ( variable.value - variable.offset - variable.min ) / variable.range - 1.0f ); 
	}
	public void SetNormalizedValue( AxisVariable axis, float value ) 
	{ 
		InputVariable variable = inputVariables[ (int) axis ];
		variable.setpoint = ( ( variable.value + 1.0f ) * variable.range / 2.0f ) + variable.offset + variable.min; 
		setpointsMask[ (int) axis ] = true;
	}

	public float GetMinValue( AxisVariable axis ) {	return inputVariables[ (int) axis ].min; }
	public float GetMaxValue( AxisVariable axis ) {	return inputVariables[ (int) axis ].max; }

	public void SetMinValue( AxisVariable axis, float value ) 
	{ 
		inputVariables[ (int) axis ].min = value;
		Calibrate( axis );
	}
	public void SetMaxValue( AxisVariable axis, float value ) 
	{ 
		inputVariables[ (int) axis ].max = value;
		Calibrate( axis );
	}

	private void Calibrate( AxisVariable axis )
	{
		InputVariable variable = inputVariables[ (int) axis ];
		variable.range = variable.max - variable.min;
		if( Mathf.Approximately( variable.range ) ) variable.range = 1.0f;
	}
		
	public void SetOffset() 
	{ 
		for( int variableIndex = 0; variableIndex < inputVariables.Length; variableIndex++ )
			inputVariables[ variableIndex ].offset = inputVariables[ variableIndex ].value; 
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
		inputVariables[ (int) AxisVariable.VELOCITY ].value = Input.GetAxis( id ) / updateTime;
		inputVariables[ (int) AxisVariable.POSITION ].value += inputVariables[ (int) AxisVariable.VELOCITY ].value * updateTime;
		inputVariables[ (int) AxisVariable.FORCE ].value = inputVariables[ (int) AxisVariable.VELOCITY ].value;
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
		inputVariables[ (int) AxisVariable.VELOCITY ].value = Input.GetAxis( id );
		inputVariables[ (int) AxisVariable.POSITION ].value += inputVariables[ (int) AxisVariable.VELOCITY ].value * updateTime;
		//feedbackPosition = position;
		inputVariables[ (int) AxisVariable.FORCE ].value = inputVariables[ (int) AxisVariable.VELOCITY ].value;
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

				for( int variableIndex = 0; variableIndex < inputVariables.Length; variableIndex++ )
					inputVariables[ variableIndex ].value = BitConverter.ToSingle( axis.inputBuffer, inputDataPosition + variableIndex * sizeof(float) );

				int outputIDPosition = 1 + axisIndex * OUTPUT_DATA_LENGTH;
				int outputMaskPosition = outputIDPosition + sizeof(byte);
				int outputDataPosition = inputIDPosition + 2 * sizeof(byte);

				axis.outputBuffer[ outputIDPosition ] = index;

				setpointsMask.CopyTo( axis.outputBuffer, outputMaskPosition );
				if( axis.outputBuffer[ outputMaskPosition ] > 0 ) axis.outputUpdated = true;
				setpointsMask.SetAll( false );

				//for( int variableIndex = 0; variableIndex < inputVariables.Length; variableIndex++ )
				//	if( ! Mathf.Approximately( inputVariables[ variableIndex ].setpoint, inputVariables[ variableIndex ].value ) ) axis.outputUpdated = true;

				if( axis.outputUpdated )
				{
					for( int variableIndex = 0; variableIndex < inputVariables.Length; variableIndex++ )
						Buffer.BlockCopy( BitConverter.GetBytes( inputVariables[ variableIndex ].setpoint ), 0, axis.outputBuffer, outputDataPosition + variableIndex * sizeof(float), sizeof(float) );
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

