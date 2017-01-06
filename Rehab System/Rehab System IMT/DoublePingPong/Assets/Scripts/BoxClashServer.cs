using UnityEngine;
using System.Collections;
using System.IO;

public class BoxClashServer : GameServer 
{
	public WaveSlaveController[] boxes = new WaveSlaveController[ 2 ];

	public override void Start()
	{
		base.Start();

		connection.Connect();

		foreach( WaveSlaveController box in boxes )
			box.enabled = true;
	}
}