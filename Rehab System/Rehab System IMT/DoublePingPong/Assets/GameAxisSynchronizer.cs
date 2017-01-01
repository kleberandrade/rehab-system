using System;

public enum GameAxisValue : int { POSITION, VELOCITY, FORCE, FEEDBACK, VALUES_NUMBER };

public abstract class GameAxisSynchronizer
{
	public const int AXIS_DATA_SIZE = GameAxisValue.VALUES_NUMBER * sizeof(float);

	public float[] outputValues = new float[ (int) GameAxisValue.VALUES_NUMBER ];
	public float[] inputValues = new float[ (int) GameAxisValue.VALUES_NUMBER ];

	public abstract void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay );
	public abstract void EncodeOutputData( byte[] outputBuffer, int dataOffset );
	public abstract void UpdateData( float updateTime );
}


public class MotionFollower : GameAxisSynchronizer
{
	public override void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay ) 
	{
		inputValues[ (int) GameAxisValue.POSITION ] = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputValues[ (int) GameAxisValue.VELOCITY ] = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
		inputValues[ (int) GameAxisValue.FORCE ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 * sizeof(float) );
	}

	public override void UpdateData( float updateTime ) {}

	public override void EncodeOutputData( byte[] outputBuffer, int dataOffset )
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.POSITION ] ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.VELOCITY ] ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.FORCE ] ), 0, outputBuffer, dataOffset + 2 * sizeof(float), sizeof(float) );
	}
}

public class MotionPredictor : MotionFollower
{
	public override void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay ) 
	{
		base.DecodeInputData( inputValues, inputBuffer, dataOffset );

		inputValues[ (int) GameAxisValue.POSITION ] += inputValues[ (int) GameAxisValue.VELOCITY ] * inputDelay;
	}

	public override void UpdateData( float updateTime )  
	{
		inputValues[ (int) GameAxisValue.POSITION ] += inputValues[ (int) GameAxisValue.VELOCITY ] * updateTime;
	}
}

public class MotionCompensator : MotionPredictor
{
	float lastMessagePosition = 0.0f;

	public override void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay ) 
	{
		base.DecodeInputData( inputValues, inputBuffer, dataOffset );

		lastMessagePosition = inputValues[ (int) GameAxisValue.POSITION ];
	}

	public override void UpdateData( float updateTime )  
	{
		float trackingError = lastMessagePosition - outputValues[ (int) GameAxisValue.FEEDBACK ];

		inputValues[ (int) GameAxisValue.VELOCITY ] += trackingError;

		base.UpdateData( updateTime );
	}
}

public abstract class WaveVariablesCompensator : GameAxisSynchronizer
{
	protected float inputWaveVariable = 0.0f, outputWaveVariable = 0.0f;
	protected float inputWaveIntegral = 0.0f, outputWaveIntegral = 0.0f;

	protected float forceIntegral = 0.0f;

	protected float waveImpedance = 10.0f;

	public override void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay ) 
	{
		inputWaveVariable = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputWaveIntegral = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
	}

	public override void EncodeOutputData( byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputWaveVariable ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputWaveIntegral ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
	}
}

public class WaveVariablesMaster : WaveVariablesCompensator
{
	public override void UpdateData( float updateTime ) 
	{
		inputValues[ (int) GameAxisValue.POSITION ] = outputValues[ (int) GameAxisValue.POSITION ];
		inputValues[ (int) GameAxisValue.VELOCITY ] = outputValues[ (int) GameAxisValue.VELOCITY ];
		inputValues[ (int) GameAxisValue.FORCE ] = waveImpedance * outputValues[ (int) GameAxisValue.VELOCITY ] - (float) Math.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;
		forceIntegral = waveImpedance * outputValues[ (int) GameAxisValue.POSITION ] - (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveIntegral;

		outputWaveVariable = -inputWaveVariable + (float) Math.Sqrt( 2 * waveImpedance ) * outputValues[ (int) GameAxisValue.VELOCITY ];
		outputWaveIntegral = -inputWaveIntegral + (float) Math.Sqrt( 2 * waveImpedance ) * outputValues[ (int) GameAxisValue.POSITION ];
	}
}

public class WaveVariablesSlave : WaveVariablesCompensator
{
	public override void UpdateData( float updateTime ) 
	{
		inputValues[ (int) GameAxisValue.POSITION ] = ( (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveIntegral - forceIntegral ) / waveImpedance;
		inputValues[ (int) GameAxisValue.VELOCITY ] = ( (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveVariable - outputValues[ (int) GameAxisValue.FORCE ] ) / waveImpedance;
		inputValues[ (int) GameAxisValue.FORCE ] = waveImpedance * ( inputValues[ (int) GameAxisValue.POSITION ] - outputValues[ (int) GameAxisValue.VELOCITY ] );

		forceIntegral += outputValues[ (int) GameAxisValue.FORCE ] * updateTime;
		outputWaveVariable = inputWaveVariable - (float) Math.Sqrt( 2 / waveImpedance ) * outputValues[ (int) GameAxisValue.FORCE ];
		outputWaveIntegral = inputWaveIntegral - (float) Math.Sqrt( 2 / waveImpedance ) * forceIntegral;
	}
}

public class SmallGainCompensator : GameAxisSynchronizer
{
	public override void DecodeInputData( byte[] inputBuffer, int dataOffset, float inputDelay ) 
	{
		inputValues[ (int) GameAxisValue.POSITION ] = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputValues[ (int) GameAxisValue.VELOCITY ] = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
		inputValues[ (int) GameAxisValue.FORCE ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 * sizeof(float) );
	}

	public override void UpdateData( float updateTime ) {}

	public override void EncodeOutputData( byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.POSITION ] ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.VELOCITY ] ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) GameAxisValue.FORCE ] ), 0, outputBuffer, dataOffset + 2 * sizeof(float), sizeof(float) );
	}
}