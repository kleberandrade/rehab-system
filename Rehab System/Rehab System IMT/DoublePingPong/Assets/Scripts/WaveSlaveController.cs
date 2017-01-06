using UnityEngine;
using System;
using System.Collections;

public class WaveSlaveController : Controller
{
	protected Vector3 outputForce = Vector3.zero;
	protected Vector3 outputForceIntegral = Vector3.zero;

	protected float waveImpedance = 10.0f;

	void Start()
	{
		body.isKinematic = false;
	}

	void FixedUpdate()
	{
		Vector3 inputWaveVariable = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, 0 ),
											     0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 ) );

		Vector3 inputWaveIntegral = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, 1 ),
												 0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 ) );

		//Vector3 inputPosition = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveIntegral - outputForceIntegral ) / waveImpedance;
		Vector3 inputVelocity = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveIntegral - outputForce ) / waveImpedance;

		//body.AddForce( waveImpedance * ( inputPosition - body.position ), ForceMode.Force );
		body.AddForce( inputVelocity - body.velocity, ForceMode.VelocityChange );

		outputForceIntegral += outputForce;
		Vector3 outputWaveVariable = inputWaveVariable - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForce;
		Vector3 outputWaveIntegral = inputWaveIntegral - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForceIntegral;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, 0, outputWaveVariable.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable.z );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, 1, outputWaveIntegral.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral.z );
	}

	void OnCollisionStay( Collision collision )
	{
		outputForce = collision.impulse / Time.fixedDeltaTime;
	}

	void OnCollisionExit( Collision collision )
	{
		outputForce = Vector3.zero;
	}
}

