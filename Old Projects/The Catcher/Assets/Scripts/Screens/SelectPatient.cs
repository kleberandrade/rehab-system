using UnityEngine;
using System.Collections;

public class SelectPatient : MonoBehaviour 
{
    private Vector2 scrollPosition;

	void OnGUI () 
    {
        SelectPlayerArea();
	}

    void SelectPlayerArea()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 600));

        GUILayout.Label("SELECT PLAYER");

        GUILayout.BeginVertical("box");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        for (int i = 0; i < 7; i++)
            GUILayout.Button("Testando " + i);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Label(string.Format("{0} patients remaining", 0));

        if (GUILayout.Button("UPDATE", GUILayout.Width(160)))
        {

        }

        if (GUILayout.Button("CANCEL", GUILayout.Width(160)))
        {

        }

        GUILayout.EndArea();
    }


}
