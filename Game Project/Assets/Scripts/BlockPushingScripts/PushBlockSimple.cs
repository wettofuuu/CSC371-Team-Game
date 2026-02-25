using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushBlockSimple : MonoBehaviour, IPushableBlock
{
    [Header("Grid Move")]
    public float gridStep = 1f;        // tile size
    public float moveTime = 0.15f;     // time to shove 1 tile
    public float spinWindow = 0.25f;   // must spin within this window

    private Rigidbody rb;
    private bool moving = false;
    private float lastConsumedSpinTime = -999f;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Keep it stable and predictable
        rb.useGravity = false; // turn ON if you actually want it to fall
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Called by your SpinPushZone (child trigger) when player is next to block
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

        StartCoroutine(MoveOneStep(new Vector3(targetX, blockPos.y, targetZ)));
    }

    IEnumerator MoveOneStep(Vector3 targetPos)
    {
        moving = true;
        if (audioSource != null) audioSource.Play();

        Vector3 start = rb.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / Mathf.Max(0.01f, moveTime);

            Vector3 next = Vector3.Lerp(start, targetPos, t);
            rb.MovePosition(next);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        moving = false;
    }

    Vector3 GetCardinalDirection(Vector3 v)
    {
        if (v.sqrMagnitude < 0.0001f) return Vector3.zero;

        if (Mathf.Abs(v.x) > Mathf.Abs(v.z))
            return v.x >= 0 ? Vector3.right : Vector3.left;
        else
            return v.z >= 0 ? Vector3.forward : Vector3.back;
    }

    float Snap(float value, float step) => Mathf.Round(value / step) * step;
}