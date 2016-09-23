using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Multiplayer : MonoBehaviour 
{
	public BallController ball;
	public BatController player;
	public BatController bat;

	public bool isPlaying = false;
	private GameConnection gameConnection = null;

	void Start()
	{
		player.enabled = true;
		bat.enabled = true;

		gameConnection = GameManager.GetGameConnection();

		StartCoroutine( WaitClients() );
	}

	IEnumerator WaitClients()
	{
		while( gameConnection.GetRemoteKeys().Length < 2 ) 
			yield return new WaitForFixedUpdate();

		Debug.Log( "enough remote keys received" );

		ball.enabled = true;
	}
}
