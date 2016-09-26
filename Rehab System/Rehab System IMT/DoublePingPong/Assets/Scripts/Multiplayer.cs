using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Multiplayer : MonoBehaviour 
{
	public BallController ball;
	public BatController[] bats = new BatController[ 4 ];

	private GameServer gameServer = null;

	void Awake()
	{
		foreach( BatController bat in bats )
			bat.enabled = true;

		gameServer = (GameServer) GameManager.GetGameConnection();
	}

	void Start()
	{
		StartCoroutine( WaitClients() );
	}

	IEnumerator WaitClients()
	{
		while( gameServer.GetClientsNumber() < 2 ) 
			yield return new WaitForFixedUpdate();

		Debug.Log( "enough remote keys received" );

		ball.enabled = true;
	}
}
