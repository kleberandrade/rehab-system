using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class InputManager : MonoBehaviour
{

	//private static StreamWriter inputLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\input.txt", false );
	//private static StreamWriter trajectoryLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\trajectory.txt", false );

	private static List<InputAxis> inputAxes = new List<InputAxis>();

	void Start()
	{
		AddAxis<MouseInputAxis>( "Mouse X", Input.mousePosition.x );
		AddAxis<MouseInputAxis>( "Mouse Y", Input.mousePosition.y );
		AddAxis<KeyboardInputAxis>( "Horizontal" );
		AddAxis<KeyboardInputAxis>( "Vertical" );

		StartCoroutine( UpdateAxisValues() );
	}

	public static InputAxis AddAxis<AxisType>( string axisID, float initialPosition = 0.0f ) where AxisType : InputAxis, new()
	{
		if( inputAxes.Find( axis => axisID == axis.Name ) == null )	inputAxes.Add( new AxisType() );

		InputAxis newAxis = GetAxis( axisID );

		if( newAxis != null ) newAxis.Init( axisID, initialPosition );

		return newAxis;
	}

	public static void RemoveAxis( string axisID )
	{
		inputAxes.RemoveAll( axis => axisID == axis.Name );
	}

	private static IEnumerator UpdateAxisValues()
	{
		while( Application.isPlaying )
		{
			float elapsedTime = Time.deltaTime;

			foreach( InputAxis inputAxis in inputAxes )
				inputAxis.Update( elapsedTime );

			yield return new WaitForSeconds( 0.005f );
		}
	}

	public static InputAxis GetAxis( string axisID )
	{
		return inputAxes.Find( axis => axisID == axis.Name );
	}

	void OnDestroy()
	{
		//inputLog.Close();
		//trajectoryLog.Close();
	}
}

