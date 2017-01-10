using UnityEngine;
using System;
using System.Collections;

public class WaveMasterController : Controller
{
	public SpringJoint spring;

	protected float waveImpedance = 10.0f;

	private float outputForce = 0.0f;
	private float outputForceIntegral = 0.0f;

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		float inputPosition = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveIntegral - outputForceIntegral ) / waveImpedance;
		float inputVelocity = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable - outputForce ) / waveImpedance;

		body.MovePosition( inputPosition * transform.forward );
		body.velocity = inputVelocity * transform.forward;

		outputForce = spring.currentForce.z;
		outputForceIntegral += outputForce * Time.fixedDeltaTime;

		float outputWaveVariable = inputWaveVariable - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForce;
		float outputWaveIntegral = inputWaveIntegral - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForceIntegral;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 2, body.position.z );
	}
}

