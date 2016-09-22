using UnityEngine;
using System.Collections;

public enum Movable { WALL = 0, BALL = 2 };

public class GameManager : MonoBehaviour 
{
	public static bool isMaster = true;

	public Gameplay gameplay;
	public Multiplayer multiplayer;

	// Use this for initialization
	void Start () 
	{
		if( isMaster ) 
		{
			multiplayer.enabled = true;
			GetComponent<GameServer>().enabled = true;
		}
		else 
		{
			gameplay.enabled = true;
			GetComponent<GameClient>().enabled = true;
		}
	}
}
