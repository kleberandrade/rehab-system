using System;

public enum NetworkValue : int { POSITION, VELOCITY, FORCE };


public abstract class NetworkOperator
{
	public abstract void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset );

	public abstract void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset );
}


public class NetworkMotionOperator : NetworkOperator
{
	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset ) 
	{
		inputValues[ (int) NetworkValue.POSITION ] = BitConverter.ToSingle( inputBuffer, dataOffset );
		inputValues[ (int) NetworkValue.VELOCITY ] = BitConverter.ToSingle( inputBuffer, dataOffset + sizeof(float) );
		inputValues[ (int) NetworkValue.FORCE ] = BitConverter.ToSingle( inputBuffer, dataOffset + 2 * sizeof(float) );
	}

	public override void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset ) 
	{
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.POSITION ] ), 0, outputBuffer, dataOffset, sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.VELOCITY ] ), 0, outputBuffer, dataOffset + sizeof(float), sizeof(float) );
		Buffer.BlockCopy( BitConverter.GetBytes( outputValues[ (int) NetworkValue.FORCE ] ), 0, outputBuffer, dataOffset + 2 * sizeof(float), sizeof(float) );
	}
}

public class NetworkMotionPredictor : NetworkMotionOperator
{
	public override void DecodeInputData( float[] inputValues, byte[] inputBuffer, int dataOffset )  
	{
		base.DecodeInputData( inputValues, inputBuffer, dataOffset );

		inputValues[ (int) NetworkValue.POSITION ] += inputValues[ (int) NetworkValue.VELOCITY ] * 0.0f;//Time.fixedDeltaTime;
	}

	public override void EncodeOutputData( float[] outputValues, byte[] outputBuffer, int dataOffset )
	{
		base.EncodeOutputData( outputValues, outputBuffer, dataOffset );
	}
}