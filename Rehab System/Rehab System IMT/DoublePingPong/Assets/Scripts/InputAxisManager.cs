using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public enum InputAxisType { Keyboard, Mouse, PXIe, AnkleBot };

public class InputAxisManager : MonoBehaviour
{
	//private static StreamWriter inputLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\input.txt", false );
	//private static StreamWriter trajectoryLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\trajectory.txt", false );

	private static List<InputAxis> inputAxes = new List<InputAxis>();

	public InputAxis GetAxis( string axisID, InputAxisType axisType = InputAxisType.Mouse )
	{
		InputAxis newAxis = inputAxes.Find( axis => axisID == axis.Name );

		if( newAxis == null ) 
		{
			if( axisType == InputAxisType.Mouse ) newAxis = new MouseInputAxis();
			else if( axisType == InputAxisType.Keyboard ) newAxis = new KeyboardInputAxis();
			else if( axisType == InputAxisType.PXIe ) newAxis = new PXIeInputAxis();
			else if( axisType == InputAxisType.AnkleBot ) newAxis = new AnkleBotInputAxis();
			else return null;

			if( !newAxis.Init( axisID ) ) return null;

			inputAxes.Add( newAxis );
		}

		return newAxis;
	}

	public void RemoveAxis( string axisID )
	{
		inputAxes.RemoveAll( axis => axisID == axis.Name );
	}

	void FixedUpdate()
	{
		float elapsedTime = Time.deltaTime;

		foreach( InputAxis inputAxis in inputAxes )
			inputAxis.Update( elapsedTime );
	}

	void OnDestroy()
	{
		//inputAxes.Clear();

		//inputLog.Close();
		//trajectoryLog.Close();
	}
}

