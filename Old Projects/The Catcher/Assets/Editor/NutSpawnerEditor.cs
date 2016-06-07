using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NutSpawner))]
public class NutSpawnerEditor: Editor
{
    private NutSpawner nutsSpawner;

	void OnEnable () 
    {
        nutsSpawner = (NutSpawner)target;
	}
	
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.backgroundColor = Color.blue;
        {
            if (GUILayout.Button("Spawn"))
            {
                nutsSpawner.Spawn();
            }
        }

    }
}
