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

		foreach( WaveMasterController box in boxes )
			box.enabled = true;
	}
}