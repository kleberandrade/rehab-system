﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SimpleJSON;

[ RequireComponent( typeof(InputAxisManager) ) ]
public class Configuration : MonoBehaviour
{
	public const string DEFAULT_IP_HOST = "127.0.0.1";

	public InputField axisServerEntry, gameServerEntry;

	public Slider[] calibrationSliders = new Slider[ 3 ];
	public Text[] valueDisplays = new Text[ 3 ];
	private float[] currentAbsoluteValues = new float[ 3 ];

	public int calibratedVariableIndex = 0;

	private static InputAxis controlAxis = null;
	private InputAxisManager axisManager = null;

	public Dropdown axisSelector;

	public float CurrentAbsoluteValue { get { return currentAbsoluteValues[ calibratedVariableIndex ]; } }

	private AxisInfoStateClient infoStateClient;

	// Use this for initialization
	void Start()
	{
		axisManager = GetComponent<InputAxisManager>();
		axisManager.ResetDefaultAxes();

		axisSelector.ClearOptions();
		axisSelector.AddOptions( InputAxisManager.DEFAULT_AXIS_NAMES );

        SetSelectedAxis( 0 );

		axisServerEntry.text = PlayerPrefs.GetString( RemoteInputAxis.AXIS_SERVER_HOST_ID, Configuration.DEFAULT_IP_HOST );
		gameServerEntry.text = PlayerPrefs.GetString( GameClient.GAME_SERVER_HOST_ID, Configuration.DEFAULT_IP_HOST );

		infoStateClient = new AxisInfoStateClient();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if( controlAxis != null ) 
		{
			currentAbsoluteValues[ 0 ] = controlAxis.Position;
			currentAbsoluteValues[ 1 ] = controlAxis.Velocity;
			currentAbsoluteValues[ 2 ] = controlAxis.Force;
		}
		else
		{
			currentAbsoluteValues[ 0 ] = 0.0f;
			currentAbsoluteValues[ 1 ] = 0.0f;
			currentAbsoluteValues[ 2 ] = 0.0f;
		}

		for( int variableIndex = 0; variableIndex < currentAbsoluteValues.Length; variableIndex++ ) 
		{
			if( calibrationSliders[ variableIndex ] ) 
				calibrationSliders[ variableIndex ].value = currentAbsoluteValues[ variableIndex ];

			if( valueDisplays[ variableIndex ] ) 
				valueDisplays[ variableIndex ].text = currentAbsoluteValues[ variableIndex ].ToString( "+#0.000;-#0.000; #0.000" );
		}
	}

	public void SetAxisServer( string serverHost )
	{
		Debug.Log( "Setting axis server host as " + serverHost );
		PlayerPrefs.SetString( RemoteInputAxis.AXIS_SERVER_HOST_ID, serverHost );
	}

	public void SetGameServer( string serverHost )
	{
		Debug.Log( "Setting game server host as " + serverHost );
		PlayerPrefs.SetString( GameClient.GAME_SERVER_HOST_ID, serverHost );
	}

    public void SetSelectedAxis( Int32 typeIndex )
    {
		controlAxis = axisManager.GetAxis( axisSelector.captionText.text );
	}

	public static InputAxis GetSelectedAxis()
	{
		return controlAxis;
	}

	public void RefreshAxesInfo()
	{
		byte[] infoBuffer = new byte[ AxisClient.BUFFER_SIZE ];

		infoStateClient.Connect( PlayerPrefs.GetString( RemoteInputAxis.AXIS_SERVER_HOST_ID, Configuration.DEFAULT_IP_HOST ), 50000 );

		infoStateClient.SendData( infoBuffer );
		if( infoStateClient.ReceiveData( infoBuffer ) )
		{
			axisManager.ResetDefaultAxes();

			axisSelector.ClearOptions();
			axisSelector.AddOptions( InputAxisManager.DEFAULT_AXIS_NAMES );				

			string infoString = Encoding.ASCII.GetString( infoBuffer );
			var remoteInfo = JSON.Parse( infoString );
			Debug.Log( "Received info: " + remoteInfo.ToString() );

			List<string> remoteAxisNames = new List<string>();
			var remoteAxesList = remoteInfo[ "axes" ].AsArray;
			for( int remoteAxisIndex = 0; remoteAxisIndex < remoteAxesList.Count; remoteAxisIndex++ )
			{
				string remoteAxisName = remoteAxesList[ remoteAxisIndex ].Value;
				axisManager.AddRemoteAxis( remoteAxisName, remoteAxisIndex.ToString() );
				remoteAxisNames.Add( remoteAxisName );
			}
			axisSelector.AddOptions( remoteAxisNames );
		}

	}

	public void SetSelectedAxisMax( int sliderIndex )
	{
		Debug.Log( "Set axis Max" );
		if( controlAxis != null ) controlAxis.MaxValue = calibrationSliders[ sliderIndex ].value;
	}

	public void SetSelectedAxisMin( int sliderIndex )
	{
		Debug.Log( "Set axis Min" );
		if( controlAxis != null ) controlAxis.MinValue = calibrationSliders[ sliderIndex ].value;
	}

	public void SetAxisOffset( bool enabled )
	{
		if( enabled ) 
		{
			Debug.Log( "Offset begin" );
			if( controlAxis.GetType() == typeof(RemoteInputAxis) ) infoStateClient.SendData( new byte[] { 1, 0, 5 } );
		}
		else 
		{
			Debug.Log( "Offset end" );
			if( controlAxis.GetType() == typeof(RemoteInputAxis) ) infoStateClient.SendData( new byte[] { 1, 0, 4 } );
			if( controlAxis != null )
			{
				controlAxis.PositionOffset = calibrationSliders[ 0 ].value;
				controlAxis.ForceOffset = calibrationSliders[ 2 ].value;
			}
		}
	}

    public void EndConfiguration()
    {
		infoStateClient.Disconnect();
		GameManager.isMaster = false;
        SceneManager.LoadScene( 1 );
    }
}

