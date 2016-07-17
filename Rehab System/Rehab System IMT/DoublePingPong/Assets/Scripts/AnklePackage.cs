using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnklePackage : MonoBehaviour {
	
	public Material mat;
	public Vector3 startVertex;
	public Vector3 mousePos;

	public RectTransform space;

	public Vector2 origin, size, point;
	private float lenght;

	public ToAnkleRobot package;

	private List<Vector2> ankleTrack;
	private float recStep;
	private float recTime;

	private float colorRate, colorAlpha;

	void Start() 
	{
		ankleTrack = new List<Vector2> ();
		colorRate = 0.1f;
		colorAlpha = 0.3f;
		recTime = 0f;
		recStep = 0.5f;
	}

	void OnGUI() 
	{
		if (!mat) 
		{
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}	
		// Start Graph plot
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.LINES);

		// Scale of space plot
		size = new Vector2 (
			space.rect.width * space.lossyScale.x,
			space.rect.height * space.lossyScale.y);

		// Center of space plot
		origin = space.position; // + size / 2;
		origin += Vector2.Scale(size, new Vector2(1, -1)) / 2;

		// Black Rectangle for space
		GL.Color(Color.black);
		RectForm (space.position, size);
		CrossForm (origin, size);

		// Blue Elipse for package
		GL.Color(Color.blue);
		point = origin + Vector2.Scale (package.origin, size);
		ElipseForm (point, Vector2.Scale(package.bases, size));
		ElipseForm (point, Vector2.Scale(package.bases, size)/package.elipseScale);
		CrossForm (point, 0.1f*size);


		// Black Dot for position
		GL.Color(Color.black);
		point = origin + Vector2.Scale (package.input, size);
		ElipseForm (point, 0.02f*size);
		ElipseForm (point, 0.03f*size);
		CrossForm (point, 0.02f*size);

/*		// Green Lines for helper
		GL.Color(new Color(0.0f, 0.4f, 0.0f, 1.0f));
		if (package.elipseSpace)
			ElipseForm (origin + Vector2.Scale (package.centerSpring, size), Vector2.Scale (package.freeSpace, size));
		else
			RectForm (origin + Vector2.Scale (package.centerSpring - new Vector2(package.freeSpace.x, -package.freeSpace.y), size), Vector2.Scale (package.freeSpace, size) * 2);
		CrossForm (origin + Vector2.Scale (package.centerSpring, size), 0.02f*size);
		*/
		// Red Dot for enemy
		GL.Color(Color.red);
		ElipseForm (origin + Vector2.Scale (package.enemyPos, size), 0.02f*size);
		CrossForm (origin + Vector2.Scale (package.enemyPos, size), 0.02f*size);

		// Record the track
		if (recTime <= 0f)
		{
			ankleTrack.Add (point - origin);
			recTime = recStep;
		} else
			recTime -= Time.deltaTime;
		
		// Black changing alpha for track
		GL.Color (Color.black);
		if (ankleTrack.Count > 1)
		{
			float aux = 1f;
			for (int i = ankleTrack.Count - 1; i > 0 ; i--)
				{
				GL.Color(new Color (0f, 0f, 0f, aux));
				Line(ankleTrack[i - 1] + origin, ankleTrack[i] + origin);
				if (aux > colorAlpha)
					aux -= colorRate;
				}
		}
		GL.End();
		GL.PopMatrix();

		if (package.enemy.GameStatus () == 4)
			ankleTrack.Clear ();
	}

	void Line(Vector2 start, Vector2 end)
	{
		Vector3 v3start = new Vector3 (
			start.x / Screen.width, 
			start.y / Screen.height, 0);

		Vector3 v3end = new Vector3 (
			end.x / Screen.width, 
			end.y / Screen.height, 0);

		GL.Vertex(v3start);
		GL.Vertex(v3end);


		}
	
	void RectForm(Vector3 startEdge, Vector3 sizes)
	{
		startEdge = new Vector3 (
			startEdge.x / Screen.width, 
			startEdge.y / Screen.height, 0);

		sizes = new Vector3 (
			sizes.x / Screen.width,
			sizes.y / Screen.height, 0);

		GL.Vertex(startEdge);
		GL.Vertex(startEdge + new Vector3(sizes.x, 0, 0));
		GL.Vertex(startEdge + new Vector3(sizes.x, 0, 0));
		GL.Vertex(startEdge + new Vector3(sizes.x, -sizes.y, 0));
		GL.Vertex(startEdge + new Vector3(sizes.x, -sizes.y, 0));
		GL.Vertex(startEdge + new Vector3(0, -sizes.y, 0));
		GL.Vertex(startEdge + new Vector3(0, -sizes.y, 0));
		GL.Vertex(startEdge);
	}

	void CrossForm (Vector3 center, Vector3 sizes)
	{
		center = new Vector3 (
			center.x / Screen.width, 
			center.y / Screen.height, 0);
		sizes = new Vector3 (
			sizes.x /2 / Screen.width,
			sizes.y /2 / Screen.height, 0);

		GL.Vertex(center + new Vector3(sizes.x, 0, 0));	
		GL.Vertex(center + new Vector3(-sizes.x, 0, 0));	
		GL.Vertex(center + new Vector3(0, sizes.y, 0));	
		GL.Vertex(center + new Vector3(0, -sizes.y, 0));	
	}

	void ElipseForm(Vector3 center, Vector3 sizes)
	{
		center = new Vector3 (
			center.x / Screen.width,
			center.y / Screen.height, 0);
		sizes = new Vector3 (
			sizes.x / Screen.width,
			sizes.y / Screen.height, 0);

		for (int i = 0; i < 360; i++)
		{
			GL.Vertex(center + new Vector3(
				Mathf.Sin(i*Mathf.Deg2Rad)*(sizes.x), 
				Mathf.Cos(i*Mathf.Deg2Rad)*(sizes.y), 0));
			GL.Vertex(center + new Vector3(
				Mathf.Sin((i + 1)*Mathf.Deg2Rad)*(sizes.x), 
				Mathf.Cos((i + 1)*Mathf.Deg2Rad)*(sizes.y), 0));
		}
	}

}
