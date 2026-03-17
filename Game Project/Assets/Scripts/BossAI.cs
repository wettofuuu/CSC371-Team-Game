using System.Collections;
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
    [SerializeField] private Rigidbody rb;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolWaitTime = 1.0f;
    [SerializeField] private float patrolPointTolerance = 1.0f;

    [Header("Detection")]
    [SerializeField] private float chaseRange = 7f;
    [SerializeField] private float loseRange = 11f;
    [SerializeField] private float killRange = 1.5f;

    [Header("Chase")]
    [SerializeField] private float repathInterval = 0.15f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private bool loadLoseScene = false;
    [SerializeField] private string loseSceneName = "LoseScene";

    [Header("Animator Parameters")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Piston Reaction")]
    [SerializeField] private float launchUpForce = 8f;
    [SerializeField] private float launchAwayForce = 3f;
    [SerializeField] private float recoverDelay = 1.1f;
    [SerializeField] private float navMeshRecoverRadius = 4f;

    private enum State
    {
        Patrol,
        Chase,
        Attack
    }

    private State currentState = State.Patrol;

    private float patrolWaitTimer = 0f;
    private float attackTimer = 0f;
    private float repathTimer = 0f;
    private Vector3 currentPatrolTarget;

    private bool isLaunched = false;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (arenaCenter == null)
        {
            arenaCenter = transform;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = true;
        }
    }

    private void Start()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            // 不要让 boss 死命贴到玩家中心
            agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, killRange * 0.8f);
            PickNewPatrolPoint();
        }
    }

    private void Update()
    {
        if (isLaunched) return;
        if (player == null || agent == null) return;
        if (!agent.isOnNavMesh) return;

        attackTimer -= Time.deltaTime;
        repathTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                if (distanceToPlayer <= killRange)
                    currentState = State.Attack;
                else if (distanceToPlayer <= chaseRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distanceToPlayer <= killRange)
                    currentState = State.Attack;
                else if (distanceToPlayer >= loseRange)
                    currentState = State.Patrol;
                break;

            case State.Attack:
                if (distanceToPlayer > killRange)
                    currentState = (distanceToPlayer <= chaseRange) ? State.Chase : State.Patrol;
                break;
        }

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Chase:
                UpdateChase(distanceToPlayer);
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

    private void UpdateChase(float distanceToPlayer)
    {
        patrolWaitTimer = 0f;

        // 接近攻击范围时停下，不要硬挤 player
        if (distanceToPlayer <= killRange + 0.15f)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        // 不要每帧都重算路径，减少抖动
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;

            if (NavMesh.SamplePosition(player.position, out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
    }

    private void UpdateAttack()
    {
        agent.isStopped = true;
        patrolWaitTimer = 0f;

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        if (attackTimer <= 0f)
        {
            if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            {
                animator.SetTrigger(attackTrigger);
            }

            KillPlayer();
            attackTimer = attackCooldown;
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

        if (loadLoseScene && !string.IsNullOrEmpty(loseSceneName))
        {
            SceneManager.LoadScene(loseSceneName);
            return;
        }

        if (player != null)
        {
            player.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.CompareTag("Piston"))
        {
            LaunchFromPiston(collision.transform.position);
        }
    }

    private void LaunchFromPiston(Vector3 pistonPosition)
    {
        if (isLaunched) return;
        if (rb == null || agent == null) return;

        isLaunched = true;

        if (agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;
        }

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 away = transform.position - pistonPosition;
        away.y = 0f;

        if (away.sqrMagnitude < 0.001f)
            away = -transform.forward;
        else
            away.Normalize();

        Vector3 launch = Vector3.up * launchUpForce + away * launchAwayForce;
        rb.AddForce(launch, ForceMode.VelocityChange);

        StartCoroutine(RecoverToNavMesh());
    }

    private IEnumerator RecoverToNavMesh()
    {
        yield return new WaitForSeconds(recoverDelay);

        float timer = 0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshRecoverRadius, NavMesh.AllAreas))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;

                transform.position = hit.position;

                agent.enabled = true;
                agent.Warp(hit.position);
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;

                isLaunched = false;
                yield break;
            }

            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.enabled = true;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        isLaunched = false;
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