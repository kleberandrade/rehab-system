using UnityEngine;

public class WavePlayerController : Controller
{
	public SpringJoint spring;

	protected float waveImpedance = 10.0f;

	private InputAxis controlAxis = null;

	void Start()
	{
		body.isKinematic = false;
		spring.spring = 0.0f;
		spring.damper = 0.0f;
	}

	void FixedUpdate()
	{
		float inputWaveVariable = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 0 );
		float inputWaveIntegral = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 1 );

		float playerForce = controlAxis.GetNormalizedValue( AxisVariable.FORCE ) * 100.0f;
		Debug.Log( "Player force: " + playerForce.ToString() );
		float feedbackForce = waveImpedance * body.velocity.z - Mathf.Sqrt( 2.0f * waveImpedance ) * inputWaveVariable;

		body.AddForce( ( playerForce + feedbackForce ) * transform.forward, ForceMode.Force );
		controlAxis.SetNormalizedValue( AxisVariable.FORCE, feedbackForce );

		float outputWaveVariable = -inputWaveVariable + Mathf.Sqrt( 2.0f * waveImpedance ) * body.velocity.z;
		float outputWaveIntegral = -inputWaveIntegral + Mathf.Sqrt( 2.0f * waveImpedance ) * body.position.z;

		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 0, outputWaveVariable );
		GameManager.GetConnection().SetLocalValue( elementID, (int) GameAxis.Z, 1, outputWaveIntegral );
	}

	public void OnEnable()
	{
		controlAxis = Configuration.GetSelectedAxis();
	}
}

