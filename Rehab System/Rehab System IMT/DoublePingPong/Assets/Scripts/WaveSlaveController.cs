using UnityEngine;

public class WaveSlaveController : Controller
{
	protected float waveImpedance = 10.0f;

	void FixedUpdate()
	{
		//float inputDelay = GameManager.GetConnection().GetNetworkDelay( elementID );

		float inputVelocity = GameManager.GetConnection().GetRemoteValue( elementID, (int) GameAxis.Z, 2 );

		body.velocity = inputVelocity * Vector3.forward;
	}
}