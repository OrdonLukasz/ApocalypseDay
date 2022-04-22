using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public ZoombieManager zoombieManager;

    public NavMeshAgent agent;
    public Transform player;
    public Transform enemyAvatar;
    public LayerMask whatIsGround, whatIsPlayer;
    public Vector2 walkPoint;

    public float footStepsRate = 0.3f;
    public float runFootStepsRate = 1f;
    public float nextFootStep = 0.7f;

    public bool walkPointSet;
    public bool alreadyAttacked;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool isEnemyDead;

    [SerializeField] private Animator animator;

    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth = 100;
    [SerializeField] private float avatarRotationFrequency = 5f;
    [SerializeField] private Vector2 xTargetPosition;
    [SerializeField] private Vector2 zTargetPosition;
    [SerializeField] private float ySpawnPosition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float checkDestinationRate = 0.25f;
    [SerializeField] private float targetAccuracy = 0.25f;

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deadSound;


    public List<Vector2> chanceTime = new List<Vector2>();
    public List<float> chance = new List<float>();
    public bool isGoingForward = false;
    public bool isGoingBack = false;
    public bool isStay = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator.SetBool("IsIdle", true);
    }

    public void Start()
    {
        SetDestination();
        StartCoroutine(CheckIfPlayerNearby());
    }

    public void Update()
    {

    }

    public IEnumerator CheckIfPlayerNearby()
    {
        if (!isEnemyDead)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange)
            {
                //SearchArea();
                StartCoroutine(randomState());
                Debug.Log("1");
                //OnIdle();
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                MoveToTarget(player.position);
            }
            if (playerInAttackRange && playerInSightRange)
            {
                OnAttack();
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(CheckIfPlayerNearby());
        }
    }

    IEnumerator randomState()
    {
        float randValue = Random.value;
        Debug.Log(randValue);
        if (randValue < chance[0])
        {
            CheckDestinationReached();
            isGoingForward = true;
            isStay = false;
            yield return new WaitForSeconds(UnityEngine.Random.Range(chanceTime[0].x, chanceTime[0].y));
        }
        else
        {
            OnIdle();
            isGoingForward = false;
            isStay = true;
            yield return new WaitForSeconds(UnityEngine.Random.Range(chanceTime[1].x, chanceTime[1].y));
        }
    }

    private void OnDestinationReached()
    {
        SetDestination();
    }

    private void CheckDestinationReached()
    {
        Debug.Log("2");
        if (Vector3.Distance(transform.position, targetPosition) <= targetAccuracy)
        {
            OnDestinationReached();
        }
    }

    private void SetDestination()
    {
        targetPosition = RandomPosition();
        //agent.SetDestination(targetPosition);
        MoveToTarget(targetPosition);
    }

    private void UpdateAvatarRotation(Vector3 lookPoint)
    {
        if (!isEnemyDead)
        {
            var lookPos = lookPoint - enemyAvatar.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            enemyAvatar.rotation = Quaternion.Slerp(enemyAvatar.rotation, rotation, Time.deltaTime * avatarRotationFrequency);
        }
    }

    private void MoveToTarget(Vector3 destinationPosition)
    {
        agent.enabled = true;
        if (agent.enabled)
        {
            AudioSource.PlayClipAtPoint(walkSound, transform.position, 1);
            agent.SetDestination(destinationPosition);
        }
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsWalking", true);
        UpdateAvatarRotation(destinationPosition);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private Vector3 RandomPosition()
    {
        float xRandomSpawnPositon = UnityEngine.Random.Range(xTargetPosition.x, xTargetPosition.y);
        float zRandomSpawnPositon = UnityEngine.Random.Range(zTargetPosition.x, zTargetPosition.y);

        return new Vector3(xRandomSpawnPositon, ySpawnPosition, zRandomSpawnPositon);
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
        if (!isEnemyDead)
        {
            AudioSource.PlayClipAtPoint(deadSound, transform.position, 0.5f);
        }
        isEnemyDead = true;
        agent.enabled = false;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("IsDeath");
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
