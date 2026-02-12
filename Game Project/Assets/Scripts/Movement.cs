using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : MonoBehaviour
{
    public Camera Camera;
    public float MoveSpeed = 5f;
    public float SpinSpeed = 180f;

    public LayerMask Ground;

    private NavMeshAgent agent;

    private bool Spinning;
    public bool IsSpinning => Spinning;

    // NEW: lets other scripts (lever / blocks) detect a spin reliably
    public float LastSpinTime { get; private set; } = -999f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = MoveSpeed;
        agent.stoppingDistance = 0.1f;
        agent.autoBraking = true;

        agent.acceleration = 12f;
        agent.angularSpeed = 720f;

        agent.autoTraverseOffMeshLink = true;
    }

    private IEnumerator StopSpinAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Spinning = false;
    }

    private void OnSpin(InputValue value)
    {
        if (!value.isPressed) return;

        Spinning = true;
        LastSpinTime = Time.time;              // <-- important
        StartCoroutine(StopSpinAfterDelay(0.15f));
    }

    private void OnMoveClick(InputValue value)
    {
        if (!value.isPressed) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, Ground, QueryTriggerInteraction.Ignore))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
            }
        }
    }

    private bool traversingLink = false;

    private IEnumerator TraverseLink()
    {
        traversingLink = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos;

        // Stop agent simulation while we manually move
        agent.isStopped = true;
        agent.updatePosition = false;

        float speed = Mathf.Max(0.1f, agent.speed);

        while (Vector3.Distance(transform.position, endPos) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // Snap cleanly + resync the agent so it doesn't "fight back"
        transform.position = endPos;
        agent.Warp(endPos);
        agent.nextPosition = endPos;

        agent.CompleteOffMeshLink();

        agent.updatePosition = true;
        agent.isStopped = false;

        traversingLink = false;
    }


    private void Update()
    {
        if (agent.speed != MoveSpeed) agent.speed = MoveSpeed;

        if (Spinning)
        {
            transform.Rotate(Vector3.up, SpinSpeed * Time.deltaTime);
        }

        if (agent.isOnOffMeshLink && !traversingLink)
        {
            StartCoroutine(TraverseLink());
        }
    }
}
 