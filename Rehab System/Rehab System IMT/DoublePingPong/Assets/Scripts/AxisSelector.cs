using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[ RequireComponent( typeof(InputAxisManager) ) ]
public class AxisSelector : MonoBehaviour
{
	public Slider[] calibrationSliders = new Slider[ 3 ];
	public Text[] valueDisplays = new Text[ 3 ];
	private float[] currentAbsoluteValues = new float[ 3 ];

	public int calibratedVariableIndex = 0;

	private static InputAxis controlAxis = null;
	private InputAxisManager axisManager = null;

	public float CurrentAbsoluteValue { get { return currentAbsoluteValues[ calibratedVariableIndex ]; } }

	// Use this for initialization
	void Start()
	{
		axisManager = GetComponent<InputAxisManager>();

        SetAxis( (int) InputAxisType.Keyboard );

		// hack
		//NetworkClientTCP infoClient = new NetworkClientTCP();
		//infoClient.Connect( PlayerPrefs.GetString( RemoteInputAxis.AXIS_SERVER_HOST, "192.168.0.66" ), 50000 );
		//infoClient.Disconnect();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if( controlAxis != null ) 
		{
			currentAbsoluteValues[ 0 ] = controlAxis.Position;
			currentAbsoluteValues[ 1 ] = controlAxis.Velocity;
			currentAbsoluteValues[ 2 ] = controlAxis.Force;

			for( int variableIndex = 0; variableIndex < currentAbsoluteValues.Length; variableIndex++ ) 
			{
				if( calibrationSliders[ variableIndex ] ) 
					calibrationSliders[ variableIndex ].value = currentAbsoluteValues[ variableIndex ];
				
				if( valueDisplays[ variableIndex ] ) 
					valueDisplays[ variableIndex ].text = currentAbsoluteValues[ variableIndex ].ToString( "+#0.000;-#0.000; #0.000" );
			}
		}
	}

    public void SetAxis( Int32 typeIndex )
    {
        if( Enum.IsDefined( typeof(InputAxisType), typeIndex ) )
        {
            InputAxisType controlAxisType = (InputAxisType) typeIndex;

            if( controlAxisType == InputAxisType.Mouse ) controlAxis = axisManager.GetAxis( "Mouse Y", InputAxisType.Mouse );
            else if( controlAxisType == InputAxisType.Keyboard ) controlAxis = axisManager.GetAxis( "Vertical", InputAxisType.Keyboard );
            else if( controlAxisType == InputAxisType.Remote ) controlAxis = axisManager.GetAxis( "0", InputAxisType.Remote );
        }
	}

	public static InputAxis GetSelectedAxis()
	{
		return controlAxis;
	}

    public void EndSelection()
    {
		GameManager.isMaster = false;
        SceneManager.LoadScene( 1 );
    }
}

