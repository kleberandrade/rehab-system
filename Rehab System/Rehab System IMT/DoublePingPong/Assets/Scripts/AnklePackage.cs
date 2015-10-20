using UnityEngine;
using System.Collections;

public class AnklePackage : MonoBehaviour {
	
	public Material mat;
	public Vector3 startVertex;
	public Vector3 mousePos;

	public RectTransform space;

	public Vector2 origin, size;
	public float lenght;

	private ToAnkleRobot package;

	void Awake()
	{
		package = GameObject.FindGameObjectWithTag("Connection").GetComponent<ToAnkleRobot>();
	}

//	void Update() 
//	{
//	}

	void OnPostRender() 
	{
		if (!mat) 
		{
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}

		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.LINES);
		GL.Color(Color.black);

		size = new Vector2 (
			space.rect.width * space.localScale.x,
			space.rect.height * space.localScale.y);

		origin = space.position; // + size / 2;
		origin += Vector2.Scale(size, new Vector2(1, -1)) / 2;

		RectForm (space.position, size);
		CrossForm (origin, size);

		GL.Color(Color.blue);

		ElipseForm (origin + Vector2.Scale(package.origin, size), Vector2.Scale(package.bases, size));
		CrossForm (origin + Vector2.Scale(package.origin, size), Vector2.Scale(package.bases, size) * 2);

		GL.Color(Color.red);

		ElipseForm (origin + Vector2.Scale(package.input, size), Vector2.Scale(new Vector2(0.05f, 0.05f), size));

//		for (int i = 0; i < 360; i++)
//		{
//			GL.Vertex(origin + new Vector3(
//				Mathf.Sin(i*Mathf.Deg2Rad)*(lenght + space.rect.width)/Screen.width, 
//				Mathf.Cos(i*Mathf.Deg2Rad)*(lenght + space.rect.height)/Screen.height, 0));
//			GL.Vertex(origin + new Vector3(
//				Mathf.Sin((i + 1)*Mathf.Deg2Rad)*(lenght + space.rect.width)/Screen.width, 
//				Mathf.Cos((i + 1)*Mathf.Deg2Rad)*(lenght + space.rect.height)/Screen.height, 0));
//		}
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
}
