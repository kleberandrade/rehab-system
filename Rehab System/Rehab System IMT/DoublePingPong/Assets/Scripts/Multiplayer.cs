using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Multiplayer : MonoBehaviour 
{
	public BallController ball;
	public BatController[] bats = new BatController[ 4 ];

	private GameServer gameServer = null;

	void Start()
	{
		gameServer = (GameServer) GameManager.GetConnection();
		gameServer.Connect();

		foreach( BatController bat in bats )
			bat.enabled = true;

		StartCoroutine( WaitClients() );
	}

	IEnumerator WaitClients()
	{
		Debug.Log( "Remote keys received " + gameServer.GetClientsNumber().ToString() );

		while( gameServer.GetClientsNumber() < 2 ) 
			yield return new WaitForFixedUpdate();

		Debug.Log( "Enough remote keys received" );

		ball.enabled = true;
	}

	public void ResetBall()
	{
		ball.enabled = false;
		ball.enabled = true;
	}
}
