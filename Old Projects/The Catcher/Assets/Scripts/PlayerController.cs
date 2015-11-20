using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator) )]
[RequireComponent (typeof (Rigidbody) )]
[RequireComponent (typeof (CapsuleCollider) )]
public class PlayerController : MonoBehaviour 
{
    public delegate void PlayerHandle(Vector3 position);
    public static PlayerHandle OnPlayerPosition;

    private RobotServer robot;
    public float meshMoveSpeed = 2.0f;
    public float animSpeed = 1.0f;
    public AudioClip footStepClip;
    [Range(0.0f, 1.0f)]
    public float volumeFootStep = 0.1f;
    public ParticleSystem dust;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    private Vector3 direction = Vector3.right;


    private Vector3 currentPosition;

    private float move = 0.0f;
    private float oldMove;


    void NutGrounded()
    {
        SetAnimTrigger("IsSad");
    }

    void NutCaptured()
    {
        SetAnimTrigger("IsHappy");
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim.layerCount > 1)
        {
            for (int i =1; i < anim.layerCount; i++)
                anim.SetLayerWeight(i, 1);
        }
        dust.enableEmission = false;
        robot = RobotServer.Instance;
    }

    void OnAnimatorMove()
    {
        if (anim)
        {
            //Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
            //transform.position += forward * anim.GetFloat("Speed") * meshMoveSpeed * Time.deltaTime;

            if (!robot.ClientConnected)
                transform.position += direction * anim.GetFloat("Speed") * meshMoveSpeed * Time.deltaTime;

            
        }
    }

    void Update()
    {
        if (robot.ClientConnected)
        {
            oldMove = move;
            move = (float)robot.RobotRequestData.Position;

            if (-move > 0) // esquerda
            {
                transform.position = new Vector3((-move * 8.0f) / Mathf.Abs(90), transform.position.y, transform.position.z);
            }
            else
            { // direita
                transform.position = new Vector3((-move * 8.0f) / Mathf.Abs(90), transform.position.y, transform.position.z);
            } 
 
        }
        else
        {
            move = Input.GetAxis("Horizontal");

        }

        direction.x = (oldMove - move);

        if ((oldMove - move) < -0.1f)
        {
            transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
            SetTransformZ(0.19f);
            anim.SetBool("Mirror", true);
        }

        if ((oldMove - move)  > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            SetTransformZ(-0.31f);
            anim.SetBool("Mirror", false);
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.WalkFwd"))
            dust.enableEmission = false;
        else
            dust.enableEmission = true;

        if (OnPlayerPosition != null)
            OnPlayerPosition(transform.position);
    }

    void FixedUpdate()
    {
        anim.SetFloat("Speed", Mathf.Abs(oldMove - move));
        anim.speed = animSpeed;
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
    }

    #region Animation Events
    void SteppedDown()
    {
        SoundManager.Instance.Play(footStepClip, volumeFootStep, false);
    }
    #endregion

    void SetTransformZ(float n)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, n);
    }

    void SetAnimTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.transform.CompareTag("Nut"))
        {
            SetAnimTrigger("IsDizzy");
        }
    }
}