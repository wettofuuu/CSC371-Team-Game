using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class SpinPushBlock : MonoBehaviour
{
    [Header("Grid Move")]
    public float gridStep = 1f;            // tile size
    public float moveTime = 0.15f;         // time to shove 1 tile
    public float spinWindow = 0.25f;       // must spin within this window

    [Header("Support Check (stop sliding over gaps)")]
    public LayerMask supportLayers;        // set to ground layers (Walkable/Default/etc.)
    public float supportRayStart = 0.6f;   // ray start above block center
    public float supportRayLength = 1.2f;  // how far down to look for support

    [Header("Nav / Bridge behavior")]
    public float droppedYDelta = 0.3f;     // how far down from start counts as "in pit"

    private Rigidbody rb;
    private bool moving = false;
    private float lastConsumedSpinTime = -999f;

    private NavMeshObstacle obstacle;
    private float startY;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); 
        rb.useGravity = true;

        // Don't rotate, but allow moving/falling
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        obstacle = GetComponent<NavMeshObstacle>();
        startY = transform.position.y;

        // If you use a NavMeshObstacle on the block, carve ONLY when stationary.
        if (obstacle != null)
        {
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
            obstacle.enabled = true; // block navmesh while on top floor
        }
    }

    // Called by SpinPushZone (child trigger) when player is next to block
    public void TryPushFromPlayer(Collider playerCollider)
    {
        if (moving) return;

        Movement movement = playerCollider.GetComponentInParent<Movement>();
        if (movement == null) return;

        float spinTime = movement.LastSpinTime;

        // Must have spun recently
        if ((Time.time - spinTime) > spinWindow) return;

        // One move per spin
        if (spinTime <= lastConsumedSpinTime) return;
        lastConsumedSpinTime = spinTime;

        // Push away from player along cardinal axis
        Vector3 playerPos = movement.transform.position;
        Vector3 blockPos = rb.position;
        playerPos.y = blockPos.y;

        Vector3 delta = (blockPos - playerPos);
        delta.y = 0f;

        Vector3 dir = GetCardinalDirection(delta);
        if (dir == Vector3.zero) return;

        float targetX = Snap(blockPos.x + dir.x * gridStep, gridStep);
        float targetZ = Snap(blockPos.z + dir.z * gridStep, gridStep);

        StartCoroutine(MoveOneStep(targetX, targetZ));
    }

    IEnumerator MoveOneStep(float targetX, float targetZ)
    {
        moving = true;
        if (audioSource != null) audioSource.Play();

        // IMPORTANT: disable obstacle while moving so carving does NOT spam recalcs (can freeze)
        if (obstacle != null) obstacle.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        // Move in small increments; if support disappears, stop horizontal motion immediately.
        float totalFixedSteps = Mathf.Max(1f, moveTime / Time.fixedDeltaTime);
        float stepPerFixed = gridStep / totalFixedSteps;
        stepPerFixed = Mathf.Clamp(stepPerFixed, 0.01f, 0.25f);

        while (true)
        {
            // If we lose support at any time, stop sliding and fall.
            if (!HasSupportUnderneath())
                break;

            Vector3 p = rb.position;

            float newX = Mathf.MoveTowards(p.x, targetX, stepPerFixed);
            float newZ = Mathf.MoveTowards(p.z, targetZ, stepPerFixed);

            rb.MovePosition(new Vector3(newX, p.y, newZ));

            // If reached target while supported, done.
            if (Mathf.Abs(newX - targetX) < 0.001f && Mathf.Abs(newZ - targetZ) < 0.001f)
                break;

            yield return new WaitForFixedUpdate();
        }

        moving = false;

        // If it dropped into the pit, keep obstacle OFF forever (so it doesn't carve bridge/navmesh/link)
        bool droppedIntoPit = transform.position.y < (startY - droppedYDelta);

        if (obstacle != null)
            obstacle.enabled = !droppedIntoPit;
    }

    bool HasSupportUnderneath()
    {
        Vector3 origin = rb.position + Vector3.up * supportRayStart;

        // If nothing selected in inspector, treat as Everything
        int mask = (supportLayers.value == 0) ? Physics.DefaultRaycastLayers : supportLayers.value;

        return Physics.Raycast(origin, Vector3.down, supportRayLength, mask, QueryTriggerInteraction.Ignore);
    }

    Vector3 GetCardinalDirection(Vector3 v)
    {
        if (v.sqrMagnitude < 0.0001f) return Vector3.zero;

        // Choose the dominant axis so it moves on-grid
        if (Mathf.Abs(v.x) > Mathf.Abs(v.z))
            return v.x >= 0 ? Vector3.right : Vector3.left;
        else
            return v.z >= 0 ? Vector3.forward : Vector3.back;
    }

    float Snap(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }
}
