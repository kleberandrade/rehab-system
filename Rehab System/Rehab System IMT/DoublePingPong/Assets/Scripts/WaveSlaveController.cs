using UnityEngine;

public class WaveSlaveController : Controller
{
	protected float waveImpedance = 10.0f;

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		float inputPosition = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveIntegral ) / waveImpedance;
		float inputVelocity = ( Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable ) / waveImpedance;

		body.MovePosition( inputPosition * transform.forward );
		body.velocity = inputVelocity * transform.forward;
	}
}