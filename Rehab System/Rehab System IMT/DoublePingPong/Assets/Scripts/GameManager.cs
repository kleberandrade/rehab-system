using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class GameState : MonoBehaviour
{
	protected GameConnectionBase connection = null;

	public GameConnectionBase GetConnection() 
	{ 
		return connection; 
	}

	IEnumerator UpdateNetworkData()
	{
		float networkDelay = 0.0f;

		while( Application.isPlaying )
		{
			networkDelay = connection.UpdateData( networkDelay );
			yield return new WaitForSecondsRealtime( networkDelay );
		}
	}
}

public abstract class GameClient : GameState
{
	public Camera gameCamera;

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores

	public Slider setpointSlider;
	protected Image sliderHandle;
}

public abstract class GameServer : GameState {}

public class GameManager : MonoBehaviour 
{
	public static bool isMaster = true;

	public GameClient client;
	public GameServer server;

	private static GameState game = null;

	void Start () 
	{
		//game = isMaster ? server : client;
		if( isMaster ) game = server;
		else game = client;

		game.enabled = true;
	}

	public static GameConnectionBase GetConnection()
	{
		return game.GetConnection();
	}

	void OnApplicationQuit()
	{
		GameConnectionBase.Shutdown();
	}
}
