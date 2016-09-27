﻿using UnityEngine;
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

		foreach( BatController bat in bats )
			bat.enabled = true;

		StartCoroutine( WaitClients() );
	}

	IEnumerator WaitClients()
	{
		Debug.Log( "Remote keys recieved " + gameServer.GetClientsNumber().ToString() );

		while( gameServer.GetClientsNumber() < 2 ) 
			yield return new WaitForFixedUpdate();

		Debug.Log( "Enough remote keys received" );

		ball.enabled = true;
	}
}