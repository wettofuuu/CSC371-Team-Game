using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public Camera Camera;
    public float MoveSpeed = 5f;
    public float SpinSpeed = 180f;

    public LayerMask Ground;

    [Header("Lives")]
    [SerializeField] private TMP_Text livesText;          // drag your TMP text here
    [SerializeField] private string loseSceneName = "LoseScene";
    public static int Lives { get; private set; } = 9;    // persists across scenes (static)
    private static bool livesInitialized = false;

    [Header("Collision / Blocking")]
    [SerializeField] private LayerMask Solid;   // include WALL layers here
    [SerializeField] private float capsuleSkin = 0.02f;

    [Header("Void / Falling")]
    [SerializeField] private float killY = -20f;
    [SerializeField] private Transform respawnPoint; // optional

    private NavMeshAgent agent;
    private Rigidbody rb;

    private Vector3 spawnPos;
    private Quaternion spawnRot;

    private bool isFalling = false;

    private bool Spinning;
    public bool IsSpinning => Spinning;
    public float LastSpinTime { get; private set; } = -999f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.35f;

    public bool CanJump { get; private set; } = false;
    private bool isJumping = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float dashCooldown = 0.6f;

    public bool CanDash { get; private set; } = false;
    private bool isDashing = false;
    private float lastDashTime = -999f;

    public GameObject Furball;

    private Quaternion baseLookRotation = Quaternion.identity;
    private float spinYaw = 0f;

    private bool traversingLink = false;

    public void EnableJump() => CanJump = true;
    public void EnableDash() => CanDash = true;

    // Call this when starting a NEW GAME (ex: MainMenu Play button)
    public static void ResetLives()
    {
        Lives = 9;
        livesInitialized = true;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = $"Lives: {Lives}";
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        spawnPos = transform.position;
        spawnRot = transform.rotation;

        agent.speed = MoveSpeed;
        agent.stoppingDistance = 0.1f;
        agent.autoBraking = true;
        agent.acceleration = 12f;
        agent.angularSpeed = 720f;

        agent.autoTraverseOffMeshLink = false;

        baseLookRotation = transform.rotation;

        rb.isKinematic = true;
        rb.useGravity = false;

        if (Solid.value == 0)
            Debug.LogWarning($"{name}: Solid LayerMask is empty. Jump/Dash will pass through walls.", this);

        // --- Lives init + UI ---
        if (!livesInitialized)
        {
            Lives = 9;
            livesInitialized = true;
        }
        UpdateLivesUI();
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
        if (isJumping || isDashing || isFalling) return;
        if (!agent.enabled) return;

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

    private void OnJump(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanJump) return;
        if (isJumping || isDashing || isFalling) return;
        if (traversingLink) return;
        if (!agent.enabled) return;

        StartCoroutine(JumpForwardArc_Swept());
    }

    private void OnDash(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanDash) return;
        if (isDashing || isJumping || isFalling) return;
        if (traversingLink) return;
        if (!agent.enabled) return;
        if (Time.time < lastDashTime + dashCooldown) return;

        StartCoroutine(DashPounce_Swept());
    }

    private void OnFurball(InputValue value)
    {
        if (!value.isPressed) return;
        if (Furball == null) return;

        StartCoroutine(SpitFurball());
    }

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
            Vector3 from = transform.position;
            Vector3 desired = Vector3.MoveTowards(from, endPos, speed * Time.deltaTime);

            transform.position = desired;
            agent.nextPosition = transform.position;

            yield return null;
        }

        transform.position = endPos;
        agent.nextPosition = endPos;

        agent.Warp(endPos);
        agent.CompleteOffMeshLink();

        agent.updatePosition = true;
        agent.isStopped = false;

        traversingLink = false;
    }

    private bool HasGroundBelow(Vector3 pos, float maxDistance = 2.5f)
    {
        Vector3 origin = pos + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, maxDistance, Ground, QueryTriggerInteraction.Ignore);
    }

    private void BeginFall()
    {
        if (isFalling) return;

        isFalling = true;

        StopAllCoroutines();
        isJumping = false;
        isDashing = false;
        traversingLink = false;
        Spinning = false;
        spinYaw = 0f;

        if (agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Respawn()
    {
        // --- Lose a life on every respawn ---
        Lives--;
        UpdateLivesUI();

        if (Lives <= 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(loseSceneName);
            return;
        }

        Vector3 pos = respawnPoint ? respawnPoint.position : spawnPos;
        Quaternion rot = respawnPoint ? respawnPoint.rotation : spawnRot;

        rb.isKinematic = false;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        transform.SetPositionAndRotation(pos, rot);
        baseLookRotation = rot;

        agent.enabled = true;
        agent.Warp(pos);
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        isFalling = false;
    }

    private void GetCapsulePoints(Vector3 atPos, out Vector3 p1, out Vector3 p2, out float radius)
    {
        CapsuleCollider cap = GetComponent<CapsuleCollider>();

        if (cap != null)
        {
            radius = cap.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            float height = cap.height * transform.lossyScale.y;
            float centerY = cap.center.y * transform.lossyScale.y;

            float cylinder = Mathf.Max(0f, height - 2f * radius);
            Vector3 center = atPos + Vector3.up * centerY;

            p1 = center + Vector3.up * (cylinder * 0.5f);
            p2 = center - Vector3.up * (cylinder * 0.5f);
            return;
        }

        radius = agent.radius;
        float h = agent.height;
        float centerAgentY = agent.baseOffset;

        float cylinder2 = Mathf.Max(0f, h - 2f * radius);
        Vector3 center2 = atPos + Vector3.up * centerAgentY;

        p1 = center2 + Vector3.up * (cylinder2 * 0.5f);
        p2 = center2 - Vector3.up * (cylinder2 * 0.5f);
    }

    private bool SweepCapsule(Vector3 from, Vector3 to, out Vector3 corrected)
    {
        Vector3 delta = to - from;
        float dist = delta.magnitude;

        if (dist < 0.0001f)
        {
            corrected = to;
            return false;
        }

        Vector3 dir = delta / dist;

        GetCapsulePoints(from, out Vector3 p1, out Vector3 p2, out float radius);

        if (Physics.CapsuleCast(p1, p2, radius, dir, out RaycastHit hit, dist, Solid, QueryTriggerInteraction.Ignore))
        {
            float move = Mathf.Max(0f, hit.distance - capsuleSkin);
            corrected = from + dir * move;
            return true;
        }

        corrected = to;
        return false;
    }

    private void PauseNavForManualMove()
    {
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void FinishManualMove()
    {
        Vector3 pos = transform.position;

        Vector3 origin = pos + Vector3.up * 2f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 5f, Ground, QueryTriggerInteraction.Ignore))
        {
            float bottomOffset = 0f;

            CapsuleCollider cap = GetComponent<CapsuleCollider>();
            if (cap != null)
            {
                float scaledHeight = cap.height * transform.lossyScale.y;
                float scaledRadius = cap.radius * transform.lossyScale.y;

                float half = scaledHeight * 0.5f;
                bottomOffset = half - scaledRadius;
            }

            pos.y = hit.point.y + bottomOffset;
            transform.position = pos;
        }

        agent.nextPosition = transform.position;

        if (!HasGroundBelow(transform.position))
        {
            BeginFall();
            return;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
        {
            Vector3 snapped = navHit.position;

            transform.position = snapped;
            agent.Warp(snapped);

            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }
        else
        {
            BeginFall();
        }
    }

    private IEnumerator JumpForwardArc_Swept()
    {
        isJumping = true;

        PauseNavForManualMove();

        Vector3 start = transform.position;

        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0f;
        forwardFlat = forwardFlat.sqrMagnitude > 0.0001f ? forwardFlat.normalized : Vector3.forward;

        Vector3 target = start + forwardFlat * jumpDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, jumpDuration);

            Vector3 desired = Vector3.Lerp(start, target, t);
            float arc = 4f * jumpHeight * t * (1f - t);
            desired.y += arc;

            Vector3 from = transform.position;

            if (SweepCapsule(from, desired, out Vector3 corrected))
            {
                transform.position = corrected;
                agent.nextPosition = corrected;
                break;
            }

            transform.position = desired;
            agent.nextPosition = desired;

            yield return null;
        }

        FinishManualMove();

        isJumping = false;
    }

    private IEnumerator DashPounce_Swept()
    {
        isDashing = true;
        lastDashTime = Time.time;

        PauseNavForManualMove();

        Vector3 start = transform.position;

        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0f;
        forwardFlat = forwardFlat.sqrMagnitude > 0.0001f ? forwardFlat.normalized : Vector3.forward;

        Vector3 target = start + forwardFlat * dashDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.001f, dashDuration);

            Vector3 desired = Vector3.Lerp(start, target, t);

            Vector3 from = transform.position;

            if (SweepCapsule(from, desired, out Vector3 corrected))
            {
                transform.position = corrected;
                agent.nextPosition = corrected;
                break;
            }

            transform.position = desired;
            agent.nextPosition = desired;

            yield return null;
        }

        FinishManualMove();

        isDashing = false;
    }

    private IEnumerator SpitFurball()
    {
        Vector3 spawnPos2 = transform.position + transform.forward * 1.0f;
        Quaternion spawnRot = transform.rotation;

        GameObject newFurball = Instantiate(Furball, spawnPos2, spawnRot);
        Rigidbody furRb = newFurball.GetComponent<Rigidbody>();
        if (furRb != null)
        {
            furRb.linearVelocity = transform.forward * 10f;
        }

        yield return new WaitForSeconds(1f);

        Destroy(newFurball);
    }

    private void UpdateLookAndSpin()
    {
        if (!isJumping && !isDashing)
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
                    baseLookRotation = Quaternion.LookRotation(dir);
            }
        }

        if (Spinning) spinYaw += SpinSpeed * Time.deltaTime;
        else spinYaw = 0f;

        transform.rotation = baseLookRotation * Quaternion.Euler(0f, spinYaw, 0f);
    }

    private void Update()
    {
        if (isFalling)
        {
            if (transform.position.y < killY)
                Respawn();

            UpdateLookAndSpin();
            return;
        }

        if (agent.enabled && agent.speed != MoveSpeed)
            agent.speed = MoveSpeed;

        UpdateLookAndSpin();

        if (agent.enabled && agent.isOnOffMeshLink && !traversingLink && !isJumping && !isDashing)
            StartCoroutine(TraverseLink());
    }
}