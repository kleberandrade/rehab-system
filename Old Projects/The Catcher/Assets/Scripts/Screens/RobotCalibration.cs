using UnityEngine;
using System.Collections;

public class RobotCalibration : MonoBehaviour 
{
    public GUISkin customSkin;
	
	void OnGUI () 
    {
        if (customSkin)
            GUI.skin = customSkin;

        InfoArea();

        ButtonArea();

        StepArea();
	}

    void InfoArea()
    {
        GUILayout.BeginArea(new Rect(Screen.width * 0.05f, Screen.height * 0.35f, 200.0f, 300.0f));
        GUILayout.BeginVertical();

        GUILayout.Box("CALIBRATION MENU");

        GUILayout.Space(30.0f);

        if (GUILayout.Button("CENTER"))
        {

        }

        if (GUILayout.Button("AMPLITUDE"))
        {

        }

        if (GUILayout.Button("CONTROLLER"))
        {

        }

        GUILayout.Label("Robot: XXXXXXXXXXXX");

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void ButtonArea()
    {
        if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.90f - 40.0f, 200.0f, 30.0f), "RESET"))
        {

        }

        if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.90f, 200.0f, 30.0f), "BACK"))
        {

        }

        if (GUI.Button(new Rect(Screen.width * 0.95f - 200.0f, Screen.height * 0.90f, 200.0f, 30.0f), "OK"))
        {

        }
    }

    void StepArea()
    {
        GUILayout.BeginArea(new Rect(Screen.width * 0.95f - 200.0f, Screen.height * 0.35f, 200.0f, 600.0f));
        GUILayout.BeginVertical();

        GUILayout.Box("STEP 1 - Center Wrist");

        GUILayout.Label("Integer porttitor eros ut eleifend feugiat. Proin hendrerit eget lacus in posuere. Aenean orci nunc, pretium a eros vehicula, tempor tempus arcu. Pellentesque quis sapien tortor. Sed commodo erat vel odio placerat consequat. In consectetur, enim nec pulvinar porttitor, arcu leo elementum libero, non gravida nisl odio non augue. Cras vitae mauris pretium est congue elementum in sit amet urna. Proin eget adipiscing orci, convallis laoreet tellus.");

        GUILayout.Space(10.0f);

        GUILayout.Box("STEP 2 - Center Amplitude");

        GUILayout.Space(10.0f);

        GUILayout.Box("STEP 3 - Adjustment Control");


        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
