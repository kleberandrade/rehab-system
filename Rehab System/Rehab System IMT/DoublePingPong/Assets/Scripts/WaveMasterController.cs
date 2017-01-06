using UnityEngine;

public class WaveMasterController : Controller
{
	protected float waveImpedance = 10.0f;

	private InputAxis controlAxis = null;

	void Start()
	{
		body.isKinematic = false;
	}

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		float inputForce = waveImpedance * body.velocity.z - Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;

		if( controlAxis != null ) inputForce += controlAxis.NormalizedForce;

		body.AddForce( new Vector3( 0.0f, 0.0f, inputForce ), ForceMode.Force );

		float outputWaveVariable = -inputWaveVariable + Mathf.Sqrt( 2.0f * waveImpedance ) * body.velocity.x;
		float outputWaveIntegral = -inputWaveIntegral + Mathf.Sqrt( 2.0f * waveImpedance ) * body.position.x;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral );
	}

	public void OnEnable()
	{
		controlAxis = Configuration.GetSelectedAxis();
	}
}

