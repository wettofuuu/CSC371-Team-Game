/*using System.Collections;
using UnityEngine;

public class LeverA5 : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Door Settings")]
    public string doorTag = "BarrierA";
    public float moveUpAmount = 2f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private Vector3[] doorDownPositions;
    private Vector3[] doorUpPositions;

    private bool isUp = false;
    private bool isMoving = false;

    // Shared between ALL levers
    private static int raisedLeverCount = 0;
    private const int MAX_RAISED = 2;

    // New: track how many levers have been activated
    private static int activatedLeverCount = 0;
    private static bool blocksTriggered = false;

    // Tracks whether THIS lever has already counted as activated
    private bool hasBeenActivated = false;

    // Call this when starting a new run (from main menu)
    public static void ResetForNewRun()
    {
        raisedLeverCount = 0;
        activatedLeverCount = 0;
        blocksTriggered = false;
        Debug.Log("LeverOpenBarrierA: ResetForNewRun() -> counters reset");
    }

    private void Start()
    {
        Debug.Log("Lever object position: " + transform.position);

        doors = GameObject.FindGameObjectsWithTag(doorTag);

        doorDownPositions = new Vector3[doors.Length];
        doorUpPositions = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorDownPositions[i] = doors[i].transform.position;
            doorUpPositions[i] = doorDownPositions[i] + new Vector3(0, -moveUpAmount, 0);
        }
    }

    /*private void OnTriggerStay(Collider other)
    {

        if (isMoving) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                StartCoroutine(FlickThenToggleDoors());
            }
        }
    }*/

/*private void OnTriggerStay(Collider other)
{
    if (other.TryGetComponent<Movement>(out var movement))
    {
        Debug.Log("Player is near the lever!");

        if (isMoving) return;

        if (movement.IsSpinning)
        {
            StartCoroutine(FlickThenToggleDoors());
        }
    }
}

private IEnumerator FlickThenToggleDoors()
{
    isMoving = true;

    // If we're about to RAISE and already at max, block
    if (!isUp && raisedLeverCount >= MAX_RAISED)
    {
        Debug.Log("Maximum raised barriers reached. raisedLeverCount=" + raisedLeverCount);
        isMoving = false;
        yield break;
    }

    // Reserve immediately so other levers can't also start
    if (!isUp)
        raisedLeverCount++;

    yield return StartCoroutine(AnimateLever());

    Vector3[] targets = isUp ? doorDownPositions : doorUpPositions;

    bool doorsMoving = true;
    while (doorsMoving)
    {
        doorsMoving = false;

        for (int i = 0; i < doors.Length; i++)
        {
            Transform door = doors[i].transform;

            if (Vector3.Distance(door.position, targets[i]) > 0.01f)
            {
                door.position = Vector3.MoveTowards(
                    door.position,
                    targets[i],
                    doorMoveSpeed * Time.deltaTime
                );
                doorsMoving = true;
            }
        }

        yield return null;
    }

    // Count this lever as activated the first time it is turned on
    if (!isUp && !hasBeenActivated)
    {
        hasBeenActivated = true;
        activatedLeverCount++;

        Debug.Log("Activated levers: " + activatedLeverCount);

        if (!blocksTriggered && activatedLeverCount >= 2)
        {
            blocksTriggered = true;
            OnTwoLeversActivated();
        }
    }

    // If we're returning to the "down" state, free the slot
    if (isUp)
        raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);

    isUp = !isUp;
    isMoving = false;

    Debug.Log("Raised lever groups: " + raisedLeverCount);
}

private IEnumerator AnimateLever()
{
    float elapsed = 0f;

    Quaternion startRot = transform.localRotation;
    Quaternion endRot = !isUp
        ? startRot * Quaternion.Euler(leverRotationOffset)
        : startRot * Quaternion.Euler(-leverRotationOffset);

    while (elapsed < leverAnimationTime)
    {
        float t = elapsed / leverAnimationTime;
        transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.localRotation = endRot;
}

private void OnTwoLeversActivated()
{
    Debug.Log("Two levers activated. Move the 4 blocks here.");

    // 
    // Move 4 blocks here later.
    // Left blank for now as requested.
}

// Safety: if the lever gets destroyed while "up", free the slot
private void OnDestroy()
{
    if (isUp)
        raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);
}
}*/


/*
using System.Collections;
using UnityEngine;

public class LeverA5 : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Door Settings")]
    public string doorTag = "BarrierA";
    public float moveUpAmount = 2f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private Vector3[] doorDownPositions;
    private Vector3[] doorUpPositions;

    private bool isUp = false;
    private bool isMoving = false;

    // Shared between ALL levers
    private static int raisedLeverCount = 0;
    private const int MAX_RAISED = 2;

    // Shared activation tracking
    private static int activatedLeverCount = 0;
    private static bool blocksTriggered = false;

    // Tracks whether THIS lever has already counted as activated
    private bool hasBeenActivated = false;

    // Call this when starting a new run
    public static void ResetForNewRun()
    {
        raisedLeverCount = 0;
        activatedLeverCount = 0;
        blocksTriggered = false;
        Debug.Log("LeverA5.ResetForNewRun() -> counters reset");
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name} Start()");
        Debug.Log($"{gameObject.name} position = {transform.position}");
        Debug.Log($"{gameObject.name} initial state -> isUp={isUp}, isMoving={isMoving}, hasBeenActivated={hasBeenActivated}");
        Debug.Log($"{gameObject.name} shared state at Start -> raisedLeverCount={raisedLeverCount}, activatedLeverCount={activatedLeverCount}, blocksTriggered={blocksTriggered}");

        doors = GameObject.FindGameObjectsWithTag(doorTag);

        Debug.Log($"{gameObject.name} found {doors.Length} door(s) with tag '{doorTag}'");

        doorDownPositions = new Vector3[doors.Length];
        doorUpPositions = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorDownPositions[i] = doors[i].transform.position;
            doorUpPositions[i] = doorDownPositions[i] + new Vector3(0, -moveUpAmount, 0);

            Debug.Log(
                $"{gameObject.name} door[{i}] = {doors[i].name}, " +
                $"downPos={doorDownPositions[i]}, upPos={doorUpPositions[i]}"
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{gameObject.name} OnTriggerEnter by: {other.name}");

        if (other.TryGetComponent<Movement>(out var movement))
        {
            Debug.Log($"{gameObject.name} detected player entering trigger. IsSpinning={movement.IsSpinning}");
        }
        else
        {
            Debug.Log($"{gameObject.name} trigger entered by object without Movement component: {other.name}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"{gameObject.name} OnTriggerStay by: {other.name}");

        if (isMoving)
        {
            Debug.Log($"{gameObject.name} ignored trigger because isMoving=true");
            return;
        }

        if (other.TryGetComponent<Movement>(out var movement))
        {
            Debug.Log($"{gameObject.name} found Movement on {other.name}. IsSpinning={movement.IsSpinning}");

            if (movement.IsSpinning)
            {
                Debug.Log($"{gameObject.name} starting FlickThenToggleDoors()");
                StartCoroutine(FlickThenToggleDoors());
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} OnTriggerStay: no Movement component found on {other.name}");
        }
    }

    private IEnumerator FlickThenToggleDoors()
    {
        Debug.Log($"{gameObject.name} FlickThenToggleDoors() START");
        Debug.Log(
            $"{gameObject.name} before toggle -> isUp={isUp}, isMoving={isMoving}, " +
            $"hasBeenActivated={hasBeenActivated}, raisedLeverCount={raisedLeverCount}, " +
            $"activatedLeverCount={activatedLeverCount}, blocksTriggered={blocksTriggered}"
        );

        isMoving = true;

        // If we're about to raise and already at max, block
        if (!isUp && raisedLeverCount >= MAX_RAISED)
        {
            Debug.Log($"{gameObject.name} blocked: maximum raised barriers reached. raisedLeverCount={raisedLeverCount}");
            isMoving = false;
            yield break;
        }

        // Reserve immediately so other levers can't also start
        if (!isUp)
        {
            raisedLeverCount++;
            Debug.Log($"{gameObject.name} reserving raised slot. raisedLeverCount now = {raisedLeverCount}");
        }

        Debug.Log($"{gameObject.name} beginning lever animation");
        yield return StartCoroutine(AnimateLever());
        Debug.Log($"{gameObject.name} finished lever animation");

        Vector3[] targets = isUp ? doorDownPositions : doorUpPositions;
        Debug.Log($"{gameObject.name} moving doors toward {(isUp ? "DOWN" : "UP")} targets");

        bool doorsMoving = true;
        while (doorsMoving)
        {
            doorsMoving = false;

            for (int i = 0; i < doors.Length; i++)
            {
                Transform door = doors[i].transform;

                if (Vector3.Distance(door.position, targets[i]) > 0.01f)
                {
                    door.position = Vector3.MoveTowards(
                        door.position,
                        targets[i],
                        doorMoveSpeed * Time.deltaTime
                    );
                    doorsMoving = true;
                }
            }

            yield return null;
        }

        Debug.Log($"{gameObject.name} finished moving doors");

        // Count this lever as activated the first time it is turned on
        if (!isUp && !hasBeenActivated)
        {
            hasBeenActivated = true;
            activatedLeverCount++;

            Debug.Log($"{gameObject.name} counted as activated for the first time");
            Debug.Log($"{gameObject.name} activatedLeverCount is now {activatedLeverCount}");

            if (!blocksTriggered && activatedLeverCount >= 2)
            {
                blocksTriggered = true;
                Debug.Log($"{gameObject.name} caused block trigger because activatedLeverCount >= 2");
                OnTwoLeversActivated();
            }
            else
            {
                Debug.Log(
                    $"{gameObject.name} did NOT trigger blocks. " +
                    $"blocksTriggered={blocksTriggered}, activatedLeverCount={activatedLeverCount}"
                );
            }
        }
        else
        {
            Debug.Log(
                $"{gameObject.name} did not increment activatedLeverCount. " +
                $"Reason -> isUp={isUp}, hasBeenActivated={hasBeenActivated}"
            );
        }

        // If we're returning to the down state, free the slot
        if (isUp)
        {
            raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);
            Debug.Log($"{gameObject.name} lowered lever, freeing raised slot. raisedLeverCount now = {raisedLeverCount}");
        }

        isUp = !isUp;
        isMoving = false;

        Debug.Log(
            $"{gameObject.name} after toggle -> isUp={isUp}, isMoving={isMoving}, " +
            $"hasBeenActivated={hasBeenActivated}, raisedLeverCount={raisedLeverCount}, " +
            $"activatedLeverCount={activatedLeverCount}, blocksTriggered={blocksTriggered}"
        );

        Debug.Log($"{gameObject.name} FlickThenToggleDoors() END");
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = !isUp
            ? startRot * Quaternion.Euler(leverRotationOffset)
            : startRot * Quaternion.Euler(-leverRotationOffset);

        Debug.Log($"{gameObject.name} AnimateLever() startRot={startRot.eulerAngles}, endRot={endRot.eulerAngles}");

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        Debug.Log($"{gameObject.name} AnimateLever() complete. Final localRotation={transform.localRotation.eulerAngles}");
    }

    private void OnTwoLeversActivated()
    {
        Debug.Log($"{gameObject.name} OnTwoLeversActivated() called");
        Debug.Log("Two levers activated. Move the 4 blocks here.");

        // TODO:
        // Move 4 blocks here later.
    }

    private void OnDestroy()
    {
        Debug.Log($"{gameObject.name} OnDestroy() called. isUp={isUp}");

        if (isUp)
        {
            raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);
            Debug.Log($"{gameObject.name} destroyed while up. raisedLeverCount now = {raisedLeverCount}");
        }
    }
}*/

/*using System.Collections;
using UnityEngine;

public class LeverA5 : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Door Settings")]
    public string doorTag = "BarrierA";
    public float moveUpAmount = 2f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private Vector3[] doorDownPositions;
    private Vector3[] doorUpPositions;

    private bool isUp = false;
    private bool isMoving = false;

    // Shared between ALL levers
    private static int raisedLeverCount = 0;
    private const int MAX_RAISED = 2;

    // Shared activation tracking
    private static int activatedLeverCount = 0;
    private static bool blocksTriggered = false;

    // Tracks whether THIS lever has already counted as activated
    private bool hasBeenActivated = false;

    // Call this when starting a new run
    public static void ResetForNewRun()
    {
        raisedLeverCount = 0;
        activatedLeverCount = 0;
        blocksTriggered = false;
        Debug.Log("LeverA5.ResetForNewRun() -> counters reset");
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name} Start()");

        doors = GameObject.FindGameObjectsWithTag(doorTag);

        doorDownPositions = new Vector3[doors.Length];
        doorUpPositions = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorDownPositions[i] = doors[i].transform.position;
            doorUpPositions[i] = doorDownPositions[i] + new Vector3(0, -moveUpAmount, 0);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isMoving) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                StartCoroutine(FlickThenToggleDoors());
            }
        }
    }

    private IEnumerator FlickThenToggleDoors()
    {
        isMoving = true;

        // Block if max raised reached
        if (!isUp && raisedLeverCount >= MAX_RAISED)
        {
            Debug.Log($"{gameObject.name} blocked: max raised reached");
            isMoving = false;
            yield break;
        }

        // Reserve slot
        if (!isUp)
            raisedLeverCount++;

        yield return StartCoroutine(AnimateLever());

        Vector3[] targets = isUp ? doorDownPositions : doorUpPositions;

        bool doorsMoving = true;
        while (doorsMoving)
        {
            doorsMoving = false;

            for (int i = 0; i < doors.Length; i++)
            {
                Transform door = doors[i].transform;

                if (Vector3.Distance(door.position, targets[i]) > 0.01f)
                {
                    door.position = Vector3.MoveTowards(
                        door.position,
                        targets[i],
                        doorMoveSpeed * Time.deltaTime
                    );
                    doorsMoving = true;
                }
            }

            yield return null;
        }

        // Count activation (only once per lever)
        if (!isUp && !hasBeenActivated)
        {
            hasBeenActivated = true;
            activatedLeverCount++;

            Debug.Log($"Activated levers: {activatedLeverCount}");

            // ✅ Trigger ONLY when exactly 2 levers are activated
            if (!blocksTriggered && activatedLeverCount == 2)
            {
                blocksTriggered = true;

                Debug.Log("Exactly 2 levers activated → move blocks now");

                // ==============================
                // TODO: MOVE YOUR 4 BLOCKS HERE
                // ==============================
            }
        }

        // Free slot if going down
        if (isUp)
            raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);

        isUp = !isUp;
        isMoving = false;
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = !isUp
            ? startRot * Quaternion.Euler(leverRotationOffset)
            : startRot * Quaternion.Euler(-leverRotationOffset);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
    }

    private void OnDestroy()
    {
        if (isUp)
            raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);
    }
*/
/*using System.Collections;
using UnityEngine;

public class LeverA5 : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Block Settings")]
    public string doorTag = "BarrierA";
    public float moveUpAmount = 2f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private Vector3[] doorDownPositions;
    private Vector3[] doorUpPositions;

    private bool isUp = false;
    private bool isMoving = false;

    private static int activatedLeverCount = 0;
    private static bool blocksTriggered = false;

    private bool hasBeenActivated = false;

    public static void ResetForNewRun()
    {
        activatedLeverCount = 0;
        blocksTriggered = false;
        Debug.Log("LeverA5.ResetForNewRun() -> counters reset");
    }

    private void Start()
    {
        doors = GameObject.FindGameObjectsWithTag(doorTag);

        doorDownPositions = new Vector3[doors.Length];
        doorUpPositions = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorDownPositions[i] = doors[i].transform.position;
            doorUpPositions[i] = doorDownPositions[i] + new Vector3(0, -moveUpAmount, 0);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isMoving) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                StartCoroutine(FlickThenToggleDoors());
            }
        }
    }

    private IEnumerator FlickThenToggleDoors()
    {
        isMoving = true;

        yield return StartCoroutine(AnimateLever());

        if (!isUp && !hasBeenActivated)
        {
            hasBeenActivated = true;
            activatedLeverCount++;

            Debug.Log("Activated levers: " + activatedLeverCount);

            if (!blocksTriggered && activatedLeverCount == 2)
            {
                blocksTriggered = true;
                Debug.Log("Exactly 2 levers activated. Moving blocks now.");

                Vector3[] targets = doorUpPositions;

                bool doorsMoving = true;
                while (doorsMoving)
                {
                    doorsMoving = false;

                    for (int i = 0; i < doors.Length; i++)
                    {
                        Transform door = doors[i].transform;

                        if (Vector3.Distance(door.position, targets[i]) > 0.01f)
                        {
                            door.position = Vector3.MoveTowards(
                                door.position,
                                targets[i],
                                doorMoveSpeed * Time.deltaTime
                            );
                            doorsMoving = true;
                        }
                    }

                    yield return null;
                }
            }
        }

        isUp = !isUp;
        isMoving = false;
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = !isUp
            ? startRot * Quaternion.Euler(leverRotationOffset)
            : startRot * Quaternion.Euler(-leverRotationOffset);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
    }
}*/

using System.Collections;
using UnityEngine;

public class LeverA5 : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Manager Reference")]
    public LeverA5Manager manager;

    private bool isUp = false;
    private bool isMoving = false;
    private bool hasBeenActivated = false;

    private void OnTriggerStay(Collider other)
    {
        if (isMoving) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                StartCoroutine(FlickLever());
            }
        }
    }

    private IEnumerator FlickLever()
    {
        isMoving = true;

        yield return StartCoroutine(AnimateLever());

        if (!isUp && !hasBeenActivated)
        {
            hasBeenActivated = true;

            if (manager != null)
            {
                manager.RegisterLeverActivation();
            }
            else
            {
                Debug.LogWarning(gameObject.name + " has no LeverA5Manager assigned.");
            }
        }

        isUp = !isUp;
        isMoving = false;
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = !isUp
            ? startRot * Quaternion.Euler(leverRotationOffset)
            : startRot * Quaternion.Euler(-leverRotationOffset);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
    }
}