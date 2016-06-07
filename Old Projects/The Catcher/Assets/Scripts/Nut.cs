using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nut : MonoBehaviour 
{
    public delegate void NutHandler();
    public static NutHandler OnGrounded;
    public static NutHandler OnCollected;

    public float speed = -0.5f;
    public float time = 1.0f;
    public int pointValue = 1;
    private bool captured;
    public float timeToFall = 1.0f;

    public AudioClip collidedOnGroundClip;
    public AudioClip collidedOnPlayerClip;
    
    private Vector3 capturedPosition;
    private Quaternion originRotate;
    private static Transform scoreIcon = null;
    private static HudPointText score;
    private float capturedTime;
    private float deltaTime;
    private Animator anim;

    private Vector3 originPosition = Vector3.zero;
    private Vector3 originPositionTemp = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private float timeTransition = 3.0f;
    private float startTime;

    void Awake()
    {
        originRotate = transform.rotation;
        if (scoreIcon == null)
            scoreIcon = GameObject.FindGameObjectWithTag("Score Icon").gameObject.transform;
        anim = GetComponentInChildren<Animator>();
    }

    IEnumerator ToFall()
    {
        yield return new WaitForSeconds(timeToFall);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ;
        targetPosition = Camera.main.WorldToViewportPoint(transform.position);
        RobotServer.Instance.GameDispatcherData.Position = (targetPosition.x * 2.0 - 1.0) * 90.0 * (-1.0);
    }

    void OnEnable()
    {
        anim.SetBool("Collided", false);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = true;
        captured = false;
        StartCoroutine("ToFall");
    }

    void FixedUpdate()
    {
        if (!captured)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, speed, 0.0f);
        }
        else
        {
            transform.position = Vector3.Lerp(capturedPosition, scoreIcon.position, (Time.time - capturedTime) / time);
            if (Vector3.Distance(transform.position, scoreIcon.position) == 0.0f)
            {
                Score.Instance.Add(pointValue);
                Destroy();
            }
        }
    }

    void Collected()
    {
        if (OnCollected != null)
            OnCollected();
        capturedPosition = transform.position + Vector3.forward * -2.0f;
        GetComponent<Collider>().enabled = false;
        captured = true;
        capturedTime = Time.time;
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.transform.CompareTag("Ground"))
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            anim.SetBool("Collided", true);
            transform.rotation = originRotate;
            transform.position = new Vector3(transform.position.x, hit.transform.position.y, transform.position.z);

            if (collidedOnGroundClip)
                SoundManager.Instance.Play(collidedOnGroundClip, 0.7f, false);

            if (OnGrounded != null)
                OnGrounded();

            Camera.main.SendMessage("Shake");
        }
    }
}
