using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpinToPush_More : MonoBehaviour
{
    [Header("Grid / Movement")]
    public float gridStep = 1f;
    public float moveTime = 0.15f;
    [Tooltip("Minimum movement per FixedUpdate step (clamped)")]
    public float minStep = 0.01f;

    [Header("Spin Window & Safety")]
    public float spinWindow = 0.25f;            // redundant w/ PushZone but kept here as guard
    private float lastConsumedSpinTime = -999f;

    [Header("Collision / Support")]
    [Tooltip("Layers considered blocking the block's destination")]
    public LayerMask blockingLayers;
    [Tooltip("Layers that count as solid support underneath")]
    public LayerMask supportLayers;

    [Header("Bridge / Pit")]
    public float droppedYDelta = 0.3f; // how far down from start counts as "in pit"
    public bool isBridgeBlock = false; // if true, block behavior for nav/bridge handled elsewhere

    [Header("Optional")]
    public AudioSource audioSource;

    private Rigidbody rb;
    private bool moving = false;
    private float startY = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        startY = transform.position.y;

        // Auto-grab AudioSource on the same GameObject if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called from PushZone_More when a player spin is detected in the zone.
    /// playerSpinTime is Movement.LastSpinTime at the moment of spin.
    /// </summary>
    public void TryPushFromPlayer(Collider playerCollider, float playerSpinTime)
    {
        if (moving) return;
        if (playerSpinTime <= lastConsumedSpinTime) return;
        if (Time.time - playerSpinTime > spinWindow) return; // expired

        lastConsumedSpinTime = playerSpinTime;

        // determine push direction using player's closest point (more stable than centers)
        Vector3 blockPos = rb.position;
        Vector3 closest = playerCollider.ClosestPoint(blockPos);
        closest.y = blockPos.y;

        Vector3 delta = blockPos - closest;
        delta.y = 0f;

        // if overlapping heavily, fallback to player transform direction
        if (delta.sqrMagnitude < 0.0001f)
        {
            var mv = playerCollider.GetComponentInParent<Movement>();
            if (mv != null)
            {
                Vector3 alt = (blockPos - mv.transform.position);
                alt.y = 0f;
                if (alt.sqrMagnitude >= 0.0001f) delta = alt.normalized;
            }
        }

        Vector3 dir = GetCardinalDirection(delta);
        if (dir == Vector3.zero) return;

        float targetX = Snap(blockPos.x + dir.x * gridStep, gridStep);
        float targetZ = Snap(blockPos.z + dir.z * gridStep, gridStep);
        Vector3 targetPos = new Vector3(targetX, blockPos.y, targetZ);

        // check destination is free before starting move
        if (!IsSpaceFree(targetPos)) return;

        StartCoroutine(MoveOneStep(targetPos));
    }

    IEnumerator MoveOneStep(Vector3 targetPos)
    {
        moving = true;

        // Play push audio EVERY time a push starts (restarts if already playing)
        if (audioSource != null)
        {
            // If you want overlap instead of restart, remove Stop() and use only PlayOneShot.
            audioSource.Stop();
            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
            else
                audioSource.Play(); // fallback if no clip is assigned (unlikely)
        }

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        float totalFixedSteps = Mathf.Max(1f, moveTime / Time.fixedDeltaTime);
        float stepPerFixed = gridStep / totalFixedSteps;
        stepPerFixed = Mathf.Clamp(stepPerFixed, minStep, 0.5f);

        int safety = 0;
        Vector3 lastPos = rb.position;

        while (true)
        {
            safety++;
            if (safety > 300) { Debug.LogWarning($"{name}: MoveOneStep safety break"); break; }

            // If no support underneath, break (stop sliding into pit)
            if (!HasSupportUnderneath()) { Debug.Log($"{name}: stopped - no support"); break; }

            Vector3 p = rb.position;
            float newX = Mathf.MoveTowards(p.x, targetPos.x, stepPerFixed);
            float newZ = Mathf.MoveTowards(p.z, targetPos.z, stepPerFixed);

            Vector3 next = new Vector3(newX, p.y, newZ);
            rb.MovePosition(next);

            // reached target?
            if (Mathf.Abs(newX - targetPos.x) < 0.001f && Mathf.Abs(newZ - targetPos.z) < 0.001f)
                break;

            // Did physics prevent movement? (blocked)
            if ((rb.position - lastPos).sqrMagnitude < 1e-7f)
            {
                Debug.Log($"{name}: stopped - blocked");
                break;
            }

            lastPos = rb.position;

            yield return new WaitForFixedUpdate();
        }

        moving = false;

        // optional: detect if dropped into pit and toggle NavMeshObstacle / other state
        bool droppedIntoPit = transform.position.y < (startY - droppedYDelta);
        if (droppedIntoPit)
        {
            // example: disable collider or change behavior
            // (left empty so you can implement per-project behavior)
        }
    }

    // grid cardinal decision (x or z)
    private Vector3 GetCardinalDirection(Vector3 v)
    {
        if (v.sqrMagnitude < 0.0001f) return Vector3.zero;
        if (Mathf.Abs(v.x) > Mathf.Abs(v.z)) return v.x >= 0 ? Vector3.right : Vector3.left;
        return v.z >= 0 ? Vector3.forward : Vector3.back;
    }

    private float Snap(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }

    /// <summary>
    /// Returns true if the block can fit at targetPos (ignores triggers).
    /// Uses the block's collider bounds as the test size (slightly reduced).
    /// </summary>
    private bool IsSpaceFree(Vector3 targetPos)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return true;

        Bounds b = col.bounds;
        Vector3 halfExtents = b.extents * 0.95f;
        Vector3 center = new Vector3(targetPos.x, b.center.y, targetPos.z);

        // test only blockingLayers if provided; else test everything except triggers
        int mask = blockingLayers.value == 0 ? ~0 : blockingLayers.value;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h != col && !h.isTrigger)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Improved support check: center + 4 corners. If any ray hits support layers, consider supported.
    /// </summary>
    private bool HasSupportUnderneath()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return true;

        Bounds b = col.bounds;
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(b.extents.x * 0.9f, 0f,  b.extents.z * 0.9f),
            new Vector3(b.extents.x * 0.9f, 0f, -b.extents.z * 0.9f),
            new Vector3(-b.extents.x * 0.9f, 0f,  b.extents.z * 0.9f),
            new Vector3(-b.extents.x * 0.9f, 0f, -b.extents.z * 0.9f),
        };

        float originY = b.min.y + 0.05f;
        float dist = b.extents.y + 0.3f;
        int mask = supportLayers.value == 0 ? Physics.DefaultRaycastLayers : supportLayers.value;

        foreach (var off in offsets)
        {
            Vector3 origin = new Vector3(b.center.x + off.x, originY, b.center.z + off.z);
            if (Physics.Raycast(origin, Vector3.down, dist, mask, QueryTriggerInteraction.Ignore))
                return true;
        }
        return false;
    }
}