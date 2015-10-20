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

	private Rigidbody m_Rigidbody;

	void Awake(){
		m_Rigidbody = GetComponent<Rigidbody> ();
		pickUpMask = LayerMask.GetMask ("PickUp");
	}


	void Start () 
	{
		Vector2 rand_aux = Random.insideUnitCircle;
		rand_aux = rand_aux.normalized;

		initialVelocity = new Vector3(rand_aux.x * speed,0f, rand_aux.y * speed);
		m_Rigidbody.velocity = initialVelocity;


		multiHitCheck = Time.time;
		pickUpTimeCount = 0f;
	}

	void FixedUpdate()
	{
		m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * speed;
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
			m_Rigidbody.velocity = new Vector3(-m_Rigidbody.position.x, 0f, -m_Rigidbody.position.z);
			return;
		}
		switch (wall)
		{
		case "Vertical":
			m_Rigidbody.velocity = new Vector3(-m_Rigidbody.velocity.x, 0f, Random.Range(-speed, speed));
			break;
		case "Horizontal": 
			m_Rigidbody.velocity = new Vector3(Random.Range(-speed, speed), 0f, -m_Rigidbody.velocity.z);
			break;
		case "Tower":
			m_Rigidbody.velocity = new Vector3(-m_Rigidbody.velocity.x, 0f, -m_Rigidbody.velocity.z);
			break;
		}
		multiHitCheck = Time.time;
		m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * speed;
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
		Ray ballTrack = new Ray(m_Rigidbody.position, m_Rigidbody.velocity.normalized);
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
		m_Rigidbody.velocity = new Vector3 (h, 0f, v);
	}
}
