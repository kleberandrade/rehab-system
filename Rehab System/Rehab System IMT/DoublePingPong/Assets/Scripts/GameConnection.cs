using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class GameConnection : MonoBehaviour 
{
	protected const int DATA_SIZE = 2 * sizeof(byte) + sizeof(float);

	protected byte[] inputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];
	protected byte[] outputBuffer = new byte[ NetworkInterface.BUFFER_SIZE ];

	protected List<NetworkClient> remoteClients = new List<NetworkClient>();
	protected Dictionary<KeyValuePair<byte,byte>,float> localPositions = new Dictionary<KeyValuePair<byte,byte>,float>();
	protected Dictionary<KeyValuePair<byte,byte>,float> remotePositions = new Dictionary<KeyValuePair<byte,byte>,float>();

	public void SetLocalPosition( byte elementID, byte axisIndex, float value ) 
	{ 
		localPositions[ new KeyValuePair<byte,byte>( elementID, axisIndex ) ] = value;
	}

	public bool HasRemoteKey( byte elementID, byte axisIndex )
	{
		return remotePositions.ContainsKey( new KeyValuePair<byte,byte>( elementID, axisIndex ) );
	}

	public float GetRemotePosition( byte elementID, byte axisIndex )
	{
		float value = 0.0f;

		remotePositions.TryGetValue( new KeyValuePair<byte,byte>( elementID, axisIndex ), out value );

		return value;
	}

	public KeyValuePair<byte,byte>[] GetRemotePositionKeys() 
	{
		return remotePositions.Keys.ToArray();
	}

	public abstract void Connect();

	// Use this for initialization
	void Start() 
	{
		Connect();
	}
	
	// Update is called once per frame
	/*void Update() 
	{
	
	}*/
}
