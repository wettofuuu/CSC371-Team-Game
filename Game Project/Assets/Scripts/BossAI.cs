using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolWaitTime = 1.0f;
    [SerializeField] private float patrolPointTolerance = 1.0f;

    [Header("Detection")]
    [SerializeField] private float chaseRange = 7f;
    [SerializeField] private float loseRange = 11f;
    [SerializeField] private float killRange = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private bool loadLoseScene = false;
    [SerializeField] private string loseSceneName = "LoseScene";

    [Header("Animator Parameters")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string attackTrigger = "Attack";

    private enum State
    {
        Patrol,
        Chase,
        Attack
    }

    private State currentState = State.Patrol;

    private float patrolWaitTimer = 0f;
    private float attackTimer = 0f;
    private Vector3 currentPatrolTarget;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (arenaCenter == null)
        {
            arenaCenter = transform;
        }
    }

    private void Start()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            PickNewPatrolPoint();
        }
    }

    private void Update()
    {
        if (player == null || agent == null) return;
        if (!agent.isOnNavMesh) return;

        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // swich mode
        if (distanceToPlayer <= killRange)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = State.Chase;
        }
        else if (distanceToPlayer >= loseRange)
        {
            currentState = State.Patrol;
        }

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Chase:
                UpdateChase();
                break;

            case State.Attack:
                UpdateAttack();
                break;
        }

        UpdateAnimator();
    }

    private void UpdatePatrol()
    {
        agent.isStopped = false;

        if (!agent.hasPath || agent.remainingDistance <= patrolPointTolerance)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                PickNewPatrolPoint();
                patrolWaitTimer = 0f;
            }
        }
    }

    private void UpdateChase()
    {
        patrolWaitTimer = 0f;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void UpdateAttack()
    {
        agent.isStopped = true;
        patrolWaitTimer = 0f;

        // face player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        // open hit/ kill player
        if (attackTimer <= 0f)
        {
            if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            {
                animator.SetTrigger(attackTrigger);
            }

            KillPlayer();
            attackTimer = attackCooldown;
        }

        // if player leave away, stop atteck
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > killRange)
        {
            currentState = (distanceToPlayer <= chaseRange) ? State.Chase : State.Patrol;
        }
    }

    private void PickNewPatrolPoint()
    {
        Vector3 center = arenaCenter.position;

        for (int i = 0; i < 20; i++)
        {
            Vector2 random2D = Random.insideUnitCircle * patrolRadius;
            Vector3 randomPoint = new Vector3(center.x + random2D.x, center.y, center.z + random2D.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                currentPatrolTarget = hit.position;
                agent.SetDestination(currentPatrolTarget);
                return;
            }
        }

        agent.SetDestination(center);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = agent.isStopped ? 0f : agent.velocity.magnitude;

        if (!string.IsNullOrEmpty(speedParam))
        {
            animator.SetFloat(speedParam, speed);
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Boss attacked / killed the player.");

        // put  loss scene
        if (loadLoseScene && !string.IsNullOrEmpty(loseSceneName))
        {
            SceneManager.LoadScene(loseSceneName);
            return;
        }

        // stop using player(testing)
        if (player != null)
        {
            player.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = arenaCenter != null ? arenaCenter.position : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, patrolRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}