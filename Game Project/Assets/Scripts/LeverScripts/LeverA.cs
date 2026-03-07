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

    private void Start()
    {
        doors = GameObject.FindGameObjectsWithTag(doorTag);

        doorDownPositions = new Vector3[doors.Length];
        doorUpPositions = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorDownPositions[i] = doors[i].transform.position;
            doorUpPositions[i] = doorDownPositions[i] + new Vector3(0, moveUpAmount, 0);
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

        // Prevent raising if already 2 raised
        if (!isUp && raisedLeverCount >= MAX_RAISED)
        {
            Debug.Log("Maximum raised barriers reached.");
            isMoving = false;
            yield break;
        }

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

        // Update shared counter
        if (isUp)
            raisedLeverCount--;
        else
            raisedLeverCount++;

        isUp = !isUp;
        isMoving = false;

        Debug.Log("Raised lever groups: " + raisedLeverCount);
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot;

        if (!isUp)
            endRot = startRot * Quaternion.Euler(leverRotationOffset);
        else
            endRot = startRot * Quaternion.Euler(-leverRotationOffset);

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