using UnityEngine;
using System.Collections;

public enum Movable { WALL = 0, BALL = 2 };

public class GameManager : MonoBehaviour 
{
	public static bool isMaster = true;

	public Gameplay gameplay;
	public Multiplayer multiplayer;

	private static GameConnection gameConnection = null;

	// Use this for initialization
	void Start () 
	{
		if( isMaster ) multiplayer.enabled = true;
		else gameplay.enabled = true;
	}

	public static GameConnection GetGameConnection()
	{
		if( gameConnection == null )
		{
			if( isMaster ) gameConnection = new GameServer();
			else gameConnection = new GameClient();
		}

		return gameConnection;
	}
}
