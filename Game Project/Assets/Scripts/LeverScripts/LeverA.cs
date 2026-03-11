using System.Collections;
using UnityEngine;

public class LeverOpenBarrierA : MonoBehaviour
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

    // Call this when starting a new run (from main menu)
    public static void ResetForNewRun()
    {
        raisedLeverCount = 0;
        Debug.Log("LeverOpenBarrierA: ResetForNewRun() -> raisedLeverCount = 0");
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

    // Safety: if the lever gets destroyed while "up", free the slot
    private void OnDestroy()
    {
        if (isUp)
            raisedLeverCount = Mathf.Max(0, raisedLeverCount - 1);
    }
}