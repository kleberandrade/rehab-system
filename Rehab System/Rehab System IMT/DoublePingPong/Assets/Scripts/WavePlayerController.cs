using UnityEngine;

public class WavePlayerController : Controller
{
	private float outputForce = 0.0f;
	private float outputForceIntegral = 0.0f;

	protected float waveImpedance = 10.0f;

	private InputAxis controlAxis = null;

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		outputForce = controlAxis.NormalizedForce;
		outputForceIntegral += outputForce * Time.fixedDeltaTime;

		float inputPosition = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveIntegral - outputForceIntegral ) / waveImpedance;
		float inputVelocity = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable - outputForce ) / waveImpedance;

		controlAxis.NormalizedForce = ( inputPosition - body.position.z ) / rangeLimits.z;

		body.velocity = inputVelocity * transform.forward;

		float outputWaveVariable = inputWaveVariable - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForce;
		float outputWaveIntegral = inputWaveIntegral - Mathf.Sqrt( 2.0f / waveImpedance ) * outputForceIntegral;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral );
	}

	public void OnEnable()
	{
		controlAxis = Configuration.GetSelectedAxis();
	}
}

