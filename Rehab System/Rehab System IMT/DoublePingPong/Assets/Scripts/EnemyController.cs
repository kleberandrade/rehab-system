using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed;
	public float timeDelay = 0.5f;

	public GameObject pickUp;
	public RaycastHit enemyTrack;

	private int pickUpMask;
	private Vector3 initialVelocity;

	private float pickUpTimeCount;
	private float multiHitCheck;


	void Start () 
	{
		Vector2 rand_aux = Random.insideUnitCircle;
		rand_aux = rand_aux.normalized;

		initialVelocity = new Vector3(rand_aux.x * speed,0f, rand_aux.y * speed);
		GetComponent<Rigidbody>().velocity = initialVelocity;

		pickUpMask = LayerMask.GetMask ("PickUp");
		multiHitCheck = Time.time;
		pickUpTimeCount = 0f;
	}

	void FixedUpdate()
	{
		GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * speed;
		pickUpTimeCount += Time.deltaTime;
		if (pickUpTimeCount > timeDelay)
		{
			enemyTrack = FindImpact(pickUpMask);
			Instantiate(pickUp, enemyTrack.point, Quaternion.identity);
			pickUpTimeCount = -100f;

		}

		if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.I))
		{
			MoveEnemy();
		}
	}

	void HitWall(string wall)
	{
		if (multiHitCheck == Time.time)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().position.x, 0f, -GetComponent<Rigidbody>().position.z);
			return;
		}
		switch (wall)
		{
		case "Vertical":
			GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().velocity.x, 0f, Random.Range(-speed, speed));
			break;
		case "Horizontal": 
			GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-speed, speed), 0f, -GetComponent<Rigidbody>().velocity.z);
			break;
		case "Tower":
			GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().velocity.x, 0f, -GetComponent<Rigidbody>().velocity.z);
			break;
		}
		multiHitCheck = Time.time;
		GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * speed;
		pickUpTimeCount = 0f;
	}

	void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag) 
		{
		case "Vertical":
		case "Horizontal":
		case "Tower":
			HitWall(other.gameObject.tag); 
			break;
		case "PickUp":
			Destroy(other.gameObject);
			break;
		case "Sky":
			Application.LoadLevel (Application.loadedLevel);
			break;
		}
	}

	public RaycastHit FindImpact(int mask)
	{
		Ray ballTrack = new Ray(GetComponent<Rigidbody>().position, GetComponent<Rigidbody>().velocity.normalized);
		RaycastHit boundaryHit;

		Physics.Raycast (ballTrack, out boundaryHit, 60f, mask);
		
		return boundaryHit;
	}

	void MoveEnemy()
	{
		float h = 0f;
		float v = 0f;

		if (Input.GetKey (KeyCode.L))
			h = 1f;
		if (Input.GetKey (KeyCode.J))
			h = -1f;
		if (Input.GetKey (KeyCode.I))
			v = 1f;
		if (Input.GetKey (KeyCode.K))
			v = -1f;
		GetComponent<Rigidbody>().velocity = new Vector3 (h, 0f, v);
	}
}
