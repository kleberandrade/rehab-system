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

	private List<Vector3> ankleTrack;

	private float colorRate, colorAlpha;

//	void Awake()
//	{
////		package = GameObject.FindGameObjectWithTag("Connection").GetComponent<ToAnkleRobot>();
//	}


	void Start() 
	{
		ankleTrack = new List<Vector3> ();
		colorRate = 0.01f;
		colorAlpha = 0.3f;
	}

	void OnPostRender() 
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
			space.rect.width * space.localScale.x,
			space.rect.height * space.localScale.y);

		// Center of space plot
		origin = space.position; // + size / 2;
		origin += Vector2.Scale(size, new Vector2(1, -1)) / 2;
		point = origin + Vector2.Scale (package.origin, size);

		// Black Rectangle for space
		GL.Color(Color.black);
		RectForm (space.position, size);
		CrossForm (origin, size);

		// Blue Elipse for package
		GL.Color(Color.blue);
		ElipseForm (point, Vector2.Scale(package.bases, size));
		ElipseForm (point, Vector2.Scale(package.bases, size)*package.elipseScale);
		CrossForm (point, 0.1f*size);

		point = origin + Vector2.Scale (package.input, size);

		// Red Dot for position
		GL.Color(Color.red);
		ElipseForm (point, 0.02f*size);

		ankleTrack.Add (new Vector3 (
			point.x / Screen.width, 
			point.y / Screen.height, 0));

		GL.Color (Color.black);


		if (ankleTrack.Count > 1)
		{
			float aux = 1f;
			for (int i = ankleTrack.Count - 1; i > 0 ; i--)
				{
				GL.Color(new Color (0f, 0f, 0f, aux));
				GL.Vertex(ankleTrack[i - 1]);
				GL.Vertex(ankleTrack[i]);
				if (aux > colorAlpha)
					aux -= colorRate;
				}
		}
		GL.End();
		GL.PopMatrix();
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

	void Vertex2(Vector2 v)
	{
		GL.Vertex (v);
	}
}
