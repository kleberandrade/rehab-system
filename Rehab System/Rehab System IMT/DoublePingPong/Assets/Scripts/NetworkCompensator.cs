using System;

public enum NetworkValue : int { POSITION, VELOCITY, FORCE, VALUES_NUMBER };

public abstract class NetworkCompensator
{
	public abstract void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset );
	public abstract void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset );
	public abstract void UpdateData( float[] inputValues, float[] outputValues, float updateTime );
}


public class MotionFollower : NetworkCompensator
{
	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset ) 
	{
		inputValues[ (int) NetworkValue.POSITION ] = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputValues[ (int) NetworkValue.VELOCITY ] = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
		inputValues[ (int) NetworkValue.FORCE ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 * sizeof(float) );
	}

	public override void UpdateData( float[] inputValues, float[] outputValues, float updateTime ) {}

	public override void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.POSITION ] ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.VELOCITY ] ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.FORCE ] ), 0, outputBuffer, dataOffset + 2 * sizeof(float), sizeof(float) );
	}
}

public class MotionPredictor : MotionFollower
{
	float lastMessagePosition = 0.0f;
	float messageUpdateTime = 0.0f;

	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset ) 
	{
		float predictedPosition = inputValues[ (int) NetworkValue.POSITION ];

		base.DecodeInputData( inputValues, inputBuffer, dataOffset );

		float trackingError = inputValues[ (int) NetworkValue.POSITION ] - predictedPosition;

		if( messageUpdateTime > 0.0f ) inputValues[ (int) NetworkValue.VELOCITY ] += trackingError / messageUpdateTime;
			
		messageUpdateTime = 0.0f;
	}

	public override void UpdateData( float[] inputValues, float[] outputValues, float updateTime )  
	{
		inputValues[ (int) NetworkValue.POSITION ] += inputValues[ (int) NetworkValue.VELOCITY ] * updateTime;

		messageUpdateTime += updateTime;
	}
}

public abstract class WaveVariablesCompensator : NetworkCompensator
{
	protected float inputWaveVariable = 0.0f, outputWaveVariable = 0.0f;
	protected float inputWaveIntegral = 0.0f, outputWaveIntegral = 0.0f;

	protected float forceIntegral = 0.0f;

	protected float waveImpedance = 10.0f;

	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset ) 
	{
		inputWaveVariable = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputWaveIntegral = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
	}

	public override void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputWaveVariable ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputWaveIntegral ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
	}
}

public class WaveVariablesMaster : WaveVariablesCompensator
{
	public override void UpdateData( float[] inputValues, float[] outputValues, float updateTime ) 
	{
		inputValues[ (int) NetworkValue.POSITION ] = outputValues[ (int) NetworkValue.POSITION ];
		inputValues[ (int) NetworkValue.VELOCITY ] = outputValues[ (int) NetworkValue.VELOCITY ];
		inputValues[ (int) NetworkValue.FORCE ] = waveImpedance * outputValues[ (int) NetworkValue.VELOCITY ] - (float) Math.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;
		forceIntegral = waveImpedance * outputValues[ (int) NetworkValue.POSITION ] - (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveIntegral;

		outputWaveVariable = -inputWaveVariable + (float) Math.Sqrt( 2 * waveImpedance ) * outputValues[ (int) NetworkValue.VELOCITY ];
		outputWaveIntegral = -inputWaveIntegral + (float) Math.Sqrt( 2 * waveImpedance ) * outputValues[ (int) NetworkValue.POSITION ];
	}
}

public class WaveVariablesSlave : WaveVariablesCompensator
{
	public override void UpdateData( float[] inputValues, float[] outputValues, float updateTime ) 
	{
		inputValues[ (int) NetworkValue.POSITION ] = ( (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveIntegral - forceIntegral ) / waveImpedance;
		inputValues[ (int) NetworkValue.VELOCITY ] = ( (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveVariable - outputValues[ (int) NetworkValue.FORCE ] ) / waveImpedance;
		inputValues[ (int) NetworkValue.FORCE ] = waveImpedance * ( inputValues[ (int) NetworkValue.POSITION ] - outputValues[ (int) NetworkValue.VELOCITY ] );

		forceIntegral += outputValues[ (int) NetworkValue.FORCE ] * updateTime;
		outputWaveVariable = inputWaveVariable - (float) Math.Sqrt( 2 / waveImpedance ) * outputValues[ (int) NetworkValue.FORCE ];
		outputWaveIntegral = inputWaveIntegral - (float) Math.Sqrt( 2 / waveImpedance ) * forceIntegral;
	}
}

public class SmallGainCompensator : NetworkCompensator
{
	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset ) 
	{
		inputValues[ (int) NetworkValue.POSITION ] = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputValues[ (int) NetworkValue.VELOCITY ] = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
		inputValues[ (int) NetworkValue.FORCE ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 * sizeof(float) );
	}

	public override void UpdateData( float[] inputValues, float[] outputValues, float updateTime ) {}

	public override void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.POSITION ] ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.VELOCITY ] ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.FORCE ] ), 0, outputBuffer, dataOffset + 2 * sizeof(float), sizeof(float) );
	}
}