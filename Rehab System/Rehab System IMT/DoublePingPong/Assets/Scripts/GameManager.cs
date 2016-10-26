using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public static bool isMaster = true;

	public Gameplay gameplay;
	public Multiplayer multiplayer;

	private static GameConnection gameConnection = null;


	void Awake()
	{
		if( gameConnection == null )
		{
			if( isMaster ) gameConnection = new GameServer();
			else gameConnection = new GameClient();
		}
	}

	void Start () 
	{
		if( isMaster ) multiplayer.enabled = true;
		else gameplay.enabled = true;
	}

	void FixedUpdate()
	{
		gameConnection.UpdateData();
	}

	IEnumerator UpdateNetworkData()
	{
		while( Application.isPlaying )
		{
			float networkDelay = gameConnection.UpdateData();
			yield return new WaitForSecondsRealtime( networkDelay );
		}
	}

	public static GameConnection GetConnection()
	{
		return gameConnection;
	}

	void OnApplicationQuit()
	{
		GameConnection.Shutdown();
	}
}
