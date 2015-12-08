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

	public GameObject PickUpTraining;

	// Use this for initialization
	void Start () 
	{
		plan = new List<State> ();
		floorMask = LayerMask.GetMask ("Training Control");
		wallMask = LayerMask.GetMask ("PickUp");
		timer = -1;

	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		
		if(Input.GetButton ("Fire1") && timer > 0)
		{
			InsertTarget();
		}


	}

	void InsertTarget()
	{
		State previousTarget;
		Ray camRay, trackRay;
		RaycastHit trainingFloor, targetWall;

		timer = -1;
		camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (plan.Count == 0)
		{
			previousTarget = new State(0.5f * Vector3.down, "");
			Debug.Log ("No plan");
		}
		else
		{
			previousTarget = plan[plan.Count - 1];
			Debug.Log ("Plan " + previousTarget.tag + " " + previousTarget.position);
		}
//		floorTraining.collider.tag;

		Physics.Raycast (camRay, out trainingFloor, rayLength, floorMask);
		Debug.Log ("HitCam " + trainingFloor.collider.tag + " " + trainingFloor.point);

		trackRay = new Ray (previousTarget.position, trainingFloor.point);
		Debug.Log (previousTarget.position + " - " + trainingFloor.point);

		Physics.Raycast (trackRay, out targetWall, rayLength, wallMask);
		Debug.Log ("HitWall " + targetWall.collider.tag + " " + targetWall.point);

		State bla;



		bla = new State (targetWall.point, targetWall.collider.tag);

		plan.Add (bla);

		Instantiate (PickUpTraining, targetWall.point, Quaternion.identity);
	}
}
