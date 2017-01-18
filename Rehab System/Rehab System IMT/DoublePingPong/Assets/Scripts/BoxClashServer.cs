using UnityEngine;
using System.Collections;
using System.IO;

public class BoxClashServer : GameServer 
{
	public WaveMasterController[] boxes = new WaveMasterController[ 2 ];

	public override void Start()
	{
		base.Start();

		connection.Connect();

		StartCoroutine( WaitClients() );
	}

	IEnumerator WaitClients()
	{
		while( connection.GetClientsNumber() < 2 ) 
			yield return new WaitForFixedUpdate();

		foreach( WaveMasterController box in boxes )
			box.enabled = true;
	}
}