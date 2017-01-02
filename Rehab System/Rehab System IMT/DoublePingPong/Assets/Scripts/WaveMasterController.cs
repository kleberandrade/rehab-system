using UnityEngine;
using System;
using System.Collections;

public class WaveMasterController : Controller
{
	protected float waveImpedance = 10.0f;

	void FixedUpdate()
	{
		Vector3 inputWaveVariable = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, 0 ),
												 0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 ) );

		Vector3 inputWaveIntegral = new Vector3( GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.X, 1 ),
												 0.0f, GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 ) );

		Vector3 inputForce = waveImpedance * body.velocity - (float) Math.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;
		Vector3 inputForceIntegral = waveImpedance * body.position - (float) Math.Sqrt( 2 * waveImpedance ) * inputWaveIntegral;

		body.AddForce( inputForce, ForceMode.Force );

		Vector3 outputWaveVariable = -inputWaveVariable + (float) Math.Sqrt( 2 * waveImpedance ) * body.velocity;
		Vector3 outputWaveIntegral = -inputWaveIntegral + (float) Math.Sqrt( 2 * waveImpedance ) * body.position;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, 0, outputWaveVariable.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable.z );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.X, 1, outputWaveIntegral.x );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral.z );
	}

	public void OnEnable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}
	public void OnDisable()
	{
		body.position = initialPosition;
		body.velocity = Vector3.zero;
	}
}

