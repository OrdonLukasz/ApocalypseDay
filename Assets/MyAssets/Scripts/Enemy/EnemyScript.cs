using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.AI;

public class EnemyScript : MonoBehaviour
{
    public ZoombieManager zoombieManager;

    [SerializeField] private Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public Transform enemyAvatar;
    public LayerMask whatIsGround, whatIsPlayer;
    public Vector2 walkPoint;

    public bool walkPointSet;
    public bool alreadyAttacked;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool isEnemyDead;

    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth = 100;
    [SerializeField] private float avatarRotationFrequency = 5f;

    public float footStepsRate = 0.3f;
    public float runFootStepsRate = 1f;
    public float nextFootStep = 0.7f;

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deadSound;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator.SetBool("IsIdle", true);
       // player.position = zoombieManager.playerPosition.position;
    }

    public void Start()
    {
        StartCoroutine(CheckIfPlayerNearby());
    }

    public void Update()
    {
        UpdateAvatarRotation();
    }

    public IEnumerator CheckIfPlayerNearby()
    {
        if (!isEnemyDead)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange)
            {
                //Patroling();
                OnIdle();

            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
            }
            if (playerInAttackRange && playerInSightRange)
            {
                // AttackPlayer();
                OnAttack();
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(CheckIfPlayerNearby());
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - new Vector3(walkPoint.x, 0, walkPoint.x);
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void UpdateAvatarRotation()
    {
        if (!isEnemyDead)
        {
            var lookPos = player.position - enemyAvatar.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            enemyAvatar.rotation = Quaternion.Slerp(enemyAvatar.rotation, rotation, Time.deltaTime * avatarRotationFrequency);
        }
    }

    private void ChasePlayer()
    {
        agent.enabled = true;
        if (agent.enabled)
        {
            AudioSource.PlayClipAtPoint(walkSound, transform.position, 1);
            agent.SetDestination(player.position);
        }
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsWalking", true);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    public void OnHit(float hitDamageValue)
    {
        if (currentHealth - hitDamageValue <= 0)
        {
            OnDead();
            return;
        }
        else
        {
            currentHealth -= hitDamageValue;
        }
        Debug.Log("Hit");
    }
    public void OnIdle()
    {
        if (Time.time > nextFootStep)
        {
            nextFootStep = Time.time + runFootStepsRate;
            agent.enabled = false;
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
            AudioSource.PlayClipAtPoint(idleSound, transform.position, 0.2f);
        }
    }

    public void OnDead()
    {
        isEnemyDead = true;
        agent.enabled = false;
        animator.SetBool("IsWalking", false);
        //agent.isStopped = true;
        //agent.ResetPath();
        AudioSource.PlayClipAtPoint(deadSound, transform.position, 0.5f);
        animator.SetTrigger("IsDeath");

    }

    public void OnPlayerSpotted()
    {

    }

    public void OnJump()
    {

    }

    public void OnAttack()
    {
        if (Time.time > nextFootStep)
        {
            nextFootStep = Time.time + runFootStepsRate;
            agent.enabled = false;
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", true);
            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
            AudioSource.PlayClipAtPoint(attackSound, transform.position, 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, attackRange);
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, sightRange);
    }
}
