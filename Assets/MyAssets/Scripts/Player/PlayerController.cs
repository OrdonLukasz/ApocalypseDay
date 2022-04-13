using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{

    public Transform cameraView;
    public Transform moveDirectionPoint;
    public NavMeshAgent agent;

    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float turnSmoothTime = 0.025f;
    [SerializeField] private Vector3 m_GroundNormal;
    [SerializeField] private Vector3 move;
    [SerializeField] private float turnSmoothVelocity;
    public float footStepsRate = 0.3f;
    public float runFootStepsRate = 1f;
    public float nextFootStep = 0.7f;

    public AudioClip walkfootStep;
    public AudioClip runfootStep;

    private void Start()
    {
        agent.updateRotation = false;
    }
    public void Update()
    {
        //CharacterRotation();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Walking(runSpeed);
        }
        else
        {
            Walking(speed);
        }
    }

    public void LateUpdate()
    {
        //cam.position = Vector3.zero;
    }
    public void Walking(float sped)
    {

        if (sped == runSpeed && Input.GetKey(KeyCode.W) || sped == runSpeed && Input.GetKey(KeyCode.A) || sped == runSpeed && Input.GetKey(KeyCode.D) || sped == runSpeed && Input.GetKey(KeyCode.S))
        {
            if (Time.time > nextFootStep)
            {
                nextFootStep = Time.time + runFootStepsRate;
                AudioSource.PlayClipAtPoint(walkfootStep, transform.position, 0.2f);
            }
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
        }
        else if (sped == speed && Input.GetKey(KeyCode.W) || sped == speed && Input.GetKey(KeyCode.A) || sped == speed && Input.GetKey(KeyCode.D) || sped == speed && Input.GetKey(KeyCode.S))
        {
            if (Time.time > nextFootStep)
            {
                nextFootStep = Time.time + footStepsRate;
                AudioSource.PlayClipAtPoint(walkfootStep, transform.position, 0.1f);
            }
            
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Transform frontDirection = cameraView;
        cameraView.LookAt(moveDirectionPoint);
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraView.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        if (direction.magnitude >= 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            GetComponent<NavMeshAgent>().Move(moveDir.normalized * sped * Time.deltaTime);
            PlayerMovement();
        }
        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            PlayerMovement();
        }
    }

    public void PlayerMovement()
    {
        if (move.magnitude > 1f) move.Normalize();
        {
            move = transform.InverseTransformDirection(move);
        }
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);

    }

}



