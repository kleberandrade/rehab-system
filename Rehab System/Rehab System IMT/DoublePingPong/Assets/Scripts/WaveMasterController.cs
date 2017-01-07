using UnityEngine;
using System;
using System.Collections;

public class WaveMasterController : Controller
{
	protected float waveImpedance = 10.0f;

	void Start()
	{
		body.isKinematic = false;
	}

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		float inputForce = waveImpedance * body.velocity.z - Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;

		body.AddForce( inputForce * transform.forward, ForceMode.Force );

		float outputWaveVariable = -inputWaveVariable + Mathf.Sqrt( 2.0f * waveImpedance ) * body.velocity.z;
		float outputWaveIntegral = -inputWaveIntegral + Mathf.Sqrt( 2.0f * waveImpedance ) * body.position.z;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral );
	}
}

