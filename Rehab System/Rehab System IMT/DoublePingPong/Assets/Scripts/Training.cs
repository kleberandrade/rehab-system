using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Training : MonoBehaviour {

	private static Vector3 startPlan = Vector3.down * 0.5f + Vector3.right * 1f;

	public class State
	{
		public Vector3 position;
		public string tag;
		public Object target;

		public State(Vector3 v, string s)
		{
			this.position = v;
			this.tag = s;
		}
		public State(Vector3 v, string s, Object g)
		{
			this.position = v;
			this.tag = s;
			this.target = g;
		}
		public void SetObject(Object g)
		{
			this.target = g;
			return;
		}

	}
	
	public List<State> plan;

	private float rayLength = 100f;
	private float timer;
	private int floorMask, wallMask;
	private float timeClick = 0.5f;
	private Vector3 prevMousePos;

	public GameObject pickUpTraining;
	public Transform vectorArrow;

//	public GameObject pickUpTrPrefab, vectorArrowPrefab;

	private Transform pickUpPos;
	private Stretchable arrow;

	// Use this for initialization
	void Start () 
	{
		plan = new List<State> ();
	//	plan.Add (new State(startPlan, ""));

		floorMask = LayerMask.GetMask ("Training Control");
		wallMask = LayerMask.GetMask ("PickUpWall");
		timer = -timeClick;
		prevMousePos = Vector3.one;

	//	pickUpTraining = Instantiate (pickUpTrPrefab);
	//	vectorArrow = Instantiate (vectorArrowPrefab).GetComponent<Transform>();

		arrow = vectorArrow.GetComponent<Stretchable> ();
		pickUpPos = pickUpTraining.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		
		if(Input.GetButton ("Fire1") && timer > 0)
		{
			InsertTarget();
		}

		SetVector();

	}

	private void InsertTarget()
	{
		timer = -timeClick;
		State trainingState;

		trainingState = FromMousePosition ();

		trainingState.SetObject (Instantiate (pickUpTraining, trainingState.position, Quaternion.identity));

		plan.Add (trainingState);
	}

	public State FromMousePosition()
	{
		State previousTarget;
		Ray camRay, trackRay;
		RaycastHit trainingFloor, targetWall;

		if (plan.Count == 0)
			previousTarget = new State(startPlan, "");
		else
			previousTarget = plan[plan.Count - 1];

		camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Physics.Raycast (camRay, out trainingFloor, rayLength, floorMask);
		
		trackRay = new Ray (previousTarget.position, trainingFloor.point - previousTarget.position);
		Physics.Raycast (trackRay, out targetWall, rayLength, wallMask);


		//Instantiate (pickUpTraining, targetWall.point, Quaternion.identity);

		return new State (
			targetWall.point, 
			targetWall.collider.tag
		);
	}

	public void SetVector()
	{
		Vector3 previousTarget;

		if (plan.Count == 0)
			previousTarget = startPlan;
		else
			previousTarget = plan[plan.Count - 1].position;
		
		Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
		if (screenRect.Contains (Input.mousePosition))
		{
			//			Debug.Log (Input.mousePosition);
			pickUpPos.position = FromMousePosition().position;
			arrow.SetForm(previousTarget , FromMousePosition().position);
			prevMousePos = FromMousePosition().position;
		}
		else 
		{
			arrow.SetForm(previousTarget, prevMousePos);
		}
	}
}
