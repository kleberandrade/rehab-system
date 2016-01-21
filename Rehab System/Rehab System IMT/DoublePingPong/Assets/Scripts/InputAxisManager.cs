using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class InputAxisManager : MonoBehaviour
{

	//private static StreamWriter inputLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\input.txt", false );
	//private static StreamWriter trajectoryLog = new StreamWriter( "c:\\Users\\Adriano\\Documents\\trajectory.txt", false );

	private List<InputAxis> inputAxes = new List<InputAxis>();

	public InputAxis GetAxis<AxisType>( string axisID ) where AxisType : InputAxis, new()
	{
		InputAxis newAxis = inputAxes.Find( axis => axisID == axis.Name );

		if( newAxis == null ) 
		{
			newAxis = new AxisType();

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
		inputAxes.Clear();

		//inputLog.Close();
		//trajectoryLog.Close();
	}
}

