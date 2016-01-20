using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ConnectionManager
{
	public const string GAME_SERVER_HOST_ID = "Game Server Host";

	public const string AXIS_SERVER_HOST_ID = "Axis Server Host";

	public const string LOCAL_SERVER_HOST = "localhost";

	private static NetworkClientTCP infoClient = null;
	public static NetworkClientTCP InfoClient
	{
		get 
		{
			if( infoClient == null ) infoClient = new NetworkClientTCP();

			return infoClient;
		}
	}

	private static NetworkClientUDP gameClient = null;
	public static NetworkClientUDP GameClient
	{
		get 
		{
			if( gameClient == null ) gameClient = new NetworkClientUDP();

			return gameClient;
		}
	}

	private static NetworkServerUDP gameServer = null;
	public static NetworkServerUDP GameServer
	{
		get 
		{
			if( gameServer == null ) gameServer = new NetworkServerUDP();

			return gameServer;
		}
	}

	private static NetworkClientUDP axisClient = null;
	public static NetworkClientUDP AxisClient
	{
		get 
		{
			if( axisClient == null ) axisClient = new NetworkClientUDP();
			
			return axisClient;
		}
	}
}

