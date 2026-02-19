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

    // lets other scripts (lever / blocks) detect a spin reliably
    public float LastSpinTime { get; private set; } = -999f;

    // ---- JUMP / LOOK ----
    [Header("Jump Settings")]
    [SerializeField] private float jumpDistance = 3f;     // how far forward the jump goes
    [SerializeField] private float jumpHeight = 2f;       // arc height
    [SerializeField] private float jumpDuration = 0.35f;  // time in air

    public bool CanJump { get; private set; } = false;
    private bool isJumping = false;

    // ---- DASH ----
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 4f;     // how far the pounce goes
    [SerializeField] private float dashDuration = 0.12f;  // very quick
    [SerializeField] private float dashCooldown = 0.6f;   // time before next dash
    public bool CanDash { get; private set; } = false;
    private bool isDashing = false;
    private float lastDashTime = -999f;

    // ---- FURBALL ----
    public GameObject Furball;

    // Call these from powerups
    public void EnableJump() => CanJump = true;
    public void EnableDash() => CanDash = true;

    // ---- LOOK + SPIN COMPOSITION ----
    private Quaternion baseLookRotation = Quaternion.identity;
    private float spinYaw = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = MoveSpeed;
        agent.stoppingDistance = 0.1f;
        agent.autoBraking = true;

        agent.acceleration = 12f;
        agent.angularSpeed = 720f;

        agent.autoTraverseOffMeshLink = true;

        baseLookRotation = transform.rotation;
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
        LastSpinTime = Time.time;
        StartCoroutine(StopSpinAfterDelay(0.15f));
    }

    private void OnMoveClick(InputValue value)
    {
        if (!value.isPressed) return;
        if (isJumping || isDashing) return;

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

    // Spacebar (Input System action named "Jump")
    private void OnJump(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanJump) return;
        if (isJumping || isDashing) return;
        if (traversingLink) return;

        StartCoroutine(JumpForwardArc());
    }

    // Dash action (bind a Dash action in your Input Actions and call this)
    private void OnDash(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanDash) return;
        if (isDashing || isJumping) return;
        if (traversingLink) return;
        if (Time.time < lastDashTime + dashCooldown) return;

        StartCoroutine(DashPounce());
    }

    private void OnFurball(InputValue value){
        if (!value.isPressed) return;
        if (Furball == null) return;
        StartCoroutine(SpitFurball());
    }

    private bool traversingLink = false;

    private IEnumerator TraverseLink()
    {
        traversingLink = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos;

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

        transform.position = endPos;
        agent.Warp(endPos);
        agent.nextPosition = endPos;

        agent.CompleteOffMeshLink();

        agent.updatePosition = true;
        agent.isStopped = false;

        traversingLink = false;
    }

    private IEnumerator JumpForwardArc()
    {
        isJumping = true;

        agent.isStopped = true;
        agent.updatePosition = false;

        Vector3 start = transform.position;

        // jump direction = where the player is currently facing (base look + spin already applied)
        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0f;
        forwardFlat = forwardFlat.normalized;

        Vector3 desiredEnd = start + forwardFlat * jumpDistance;

        Vector3 end = desiredEnd;
        if (NavMesh.SamplePosition(desiredEnd, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
            end = navHit.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, jumpDuration);

            Vector3 pos = Vector3.Lerp(start, end, t);

            float height = 4f * jumpHeight * t * (1f - t);
            pos.y += height;

            transform.position = pos;
            yield return null;
        }

        transform.position = end;

        agent.Warp(end);
        agent.nextPosition = end;

        agent.updatePosition = true;
        agent.isStopped = false;

        isJumping = false;
    }

    private IEnumerator SpitFurball()
    {

        Vector3 SpawnPosition = transform.position + transform.forward * 1.0f;
        Quaternion SpawnRotation = transform.rotation;

        GameObject NewFurball = Instantiate(Furball, SpawnPosition, SpawnRotation);
        Rigidbody Rb = NewFurball.GetComponent<Rigidbody>();
        if (Rb != null)
        {
            Rb.linearVelocity = transform.forward * 10f;
        }

        yield return new WaitForSeconds(1f);

        Destroy(NewFurball);
    }

    private IEnumerator DashPounce()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // Pause agent while manually moving
        agent.isStopped = true;
        agent.updatePosition = false;

        Vector3 start = transform.position;

        // dash direction = forward facing (no arc)
        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0f;
        forwardFlat = forwardFlat.normalized;

        Vector3 desiredEnd = start + forwardFlat * dashDistance;

        // If you want dash to ignore small obstacles and go as far as possible on navmesh,
        // sample a path along the ray. For now, try to land on navmesh near desiredEnd.
        Vector3 end = desiredEnd;
        if (NavMesh.SamplePosition(desiredEnd, out NavMeshHit navHit, 2.5f, NavMesh.AllAreas))
            end = navHit.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.001f, dashDuration);

            Vector3 pos = Vector3.Lerp(start, end, t);
            transform.position = pos;
            yield return null;
        }

        // finish at end, resync agent
        transform.position = end;
        agent.Warp(end);
        agent.nextPosition = end;

        agent.updatePosition = true;
        agent.isStopped = false;

        isDashing = false;
    }

    private void Update()
    {
        if (agent.speed != MoveSpeed) agent.speed = MoveSpeed;

        // ---- Update base look rotation toward mouse (yaw only) ----
        if (!isJumping && !isDashing) // don't change base look mid-jump/dash
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f, Ground, QueryTriggerInteraction.Ignore))
            {
                Vector3 lookPoint = hit.point;
                lookPoint.y = transform.position.y;

                Vector3 dir = (lookPoint - transform.position);
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.0001f)
                {
                    baseLookRotation = Quaternion.LookRotation(dir);
                }
            }
        }

        // ---- Spin is an offset on top of the base look ----
        if (Spinning)
        {
            spinYaw += SpinSpeed * Time.deltaTime;
        }
        else
        {
            spinYaw = 0f;
        }

        // Apply combined rotation (look + spin)
        transform.rotation = baseLookRotation * Quaternion.Euler(0f, spinYaw, 0f);

        if (agent.isOnOffMeshLink && !traversingLink && !isJumping && !isDashing)
        {
            StartCoroutine(TraverseLink());
        }
    }
}
