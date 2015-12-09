using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Training : MonoBehaviour {

	public class State
	{
		public Vector3 position;
		public string tag;
		
		public State(Vector3 v, string s)
		{
			this.position = v;
			this.tag = s;
		}
	}
	
	private List<State> plan;
	private float rayLength = 100f;
	private float timer;
	private int floorMask, wallMask;
	private float timeClick = 0.5f;

	public GameObject PickUpTraining;
	public Transform PickUpSetting, VectorSetting;
	public Stretchable Arrow;

	// Use this for initialization
	void Start () 
	{
		plan = new List<State> ();
		floorMask = LayerMask.GetMask ("Training Control");
		wallMask = LayerMask.GetMask ("PickUpWall");
		timer = -timeClick;
		plan.Add (new State(Vector3.zero, ""));

	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		
		if(Input.GetButton ("Fire1") && timer > 0)
		{
			InsertTarget();
		}

		Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
		if (screenRect.Contains (Input.mousePosition))
		{
//			Debug.Log (Input.mousePosition);
			PickUpSetting.position = FromMousePosition().position;
//		Arrow.SetForm(plan[plan.Count - 1].position , FromMousePosition().position);
		}
	}

	private void InsertTarget()
	{
		timer = -timeClick;
		State trainingState;

		trainingState = FromMousePosition ();

		plan.Add (trainingState);

		Instantiate (PickUpTraining, trainingState.position, Quaternion.identity);
	}

	public State FromMousePosition()
	{
		State previousTarget;
		Ray camRay, trackRay;
		RaycastHit trainingFloor, targetWall;
	
		previousTarget = plan[plan.Count - 1];
		Debug.Log ("Previous Plan " + previousTarget.tag + " - " + previousTarget.position);

		camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Debug.Log ("CamRay " + camRay.origin + " - " + camRay.direction);

		Physics.Raycast (camRay, out trainingFloor, rayLength, floorMask);
		Debug.Log ("Hit on Floor " + trainingFloor.collider.tag + " - " + trainingFloor.point);
		
		trackRay = new Ray (previousTarget.position, trainingFloor.point - previousTarget.position);
		Debug.Log ("TrackRay " + trackRay.origin + " - " + trackRay.direction);

		Physics.Raycast (trackRay, out targetWall, rayLength, wallMask);
		Debug.Log ("Hit on Wall: " + targetWall.collider.tag + " - " + targetWall.point);
		

		return new State (targetWall.point, targetWall.collider.tag);
	}
}
