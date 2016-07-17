using UnityEngine;
using System.Collections;

public class EnemyTarget : MonoBehaviour
{

	private Rigidbody rigidBody;
	private LayerMask pickUpMask;
	private EnemyController enemy;
	public bool isActive;

	void Start()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Awake()
	{
		isActive = false;
		pickUpMask = LayerMask.GetMask ("PickUpWall");
		enemy = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<EnemyController> ();
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (isActive)
		{
			rigidBody.position = Vector3.ProjectOnPlane (enemy.FindImpact (pickUpMask).point, Vector3.up) + 0.5f * Vector3.down;
			transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		} else
		{
			rigidBody.position = 100f * Vector3.forward;
			transform.rotation = Quaternion.Euler(340f, 45f, 188f);
		}
	


	//	transform.localScale = Vector3.one * (Mathf.Clamp(RandGaus() * 0.075f, 0f, 0.5f) + 0.6f);

	//	if ((transform.localScale.magnitude < 0.5f) || (transform.localScale.magnitude > 0.8f))
	//		Debug.Log (transform.localScale);
	}

/*	float RandGaus ()
	{
		Vector2 vec = Vector2.zero;
		while ((vec == Vector2.zero) || (vec.magnitude == 1f))
			vec = Random.insideUnitCircle;
		float w = vec.magnitude * vec.magnitude;

		return vec.x * Mathf.Sqrt (-2f * Mathf.Log (w) / w);
	}*/
}
