using System.Collections;
using UnityEngine;

/*public class LeverA : MonoBehaviour
{
    [Header("Snap-to transform (local by default)")]
    public bool useLocalTransform = true;

    public Vector3 snappedPosition = new Vector3(-0.5f, -0.15f, 0f);
    public Vector3 snappedRotationEuler = new Vector3(0f, 0f, -40f);

    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;

    [Header("Door Settings")]
    public string doorTag = "BarrierA";   // Tag to search for
    public float targetZ = -0.5f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private bool activated = false;

    private void Start()
    {
        // Find all doors with the tag
        doors = GameObject.FindGameObjectsWithTag(doorTag);
        Debug.Log("Found BarrierA!");
    }

    private void OnTriggerStay(Collider other)
    {
        if (activated) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                activated = true;
                StartCoroutine(FlickThenOpenDoor());
            }
        }
    }

    private IEnumerator FlickThenOpenDoor()
    {
        yield return StartCoroutine(AnimateLever());

        while (true)
        {
            bool allDoorsFinished = true;

            foreach (GameObject door in doors)
            {
                Vector3 targetPosition = new Vector3(
                    door.transform.position.x,
                    door.transform.position.y,
                    targetZ
                );

                if (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
                {
                    door.transform.position = Vector3.MoveTowards(
                        door.transform.position,
                        targetPosition,
                        doorMoveSpeed * Time.deltaTime
                    );

                    allDoorsFinished = false;
                }
            }

            if (allDoorsFinished)
                break;

            yield return null;
        }
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Vector3 startPos = useLocalTransform ? transform.localPosition : transform.position;
        Quaternion startRot = useLocalTransform ? transform.localRotation : transform.rotation;

        Vector3 endPos = snappedPosition;
        Quaternion endRot = Quaternion.Euler(snappedRotationEuler);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;

            if (useLocalTransform)
            {
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            }
            else
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (useLocalTransform)
        {
            transform.localPosition = endPos;
            transform.localRotation = endRot;
        }
        else
        {
            transform.position = endPos;
            transform.rotation = endRot;
        }
    }
}*/

/*
public class LeverOpenBarrierA : MonoBehaviour
{
    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;
    public Vector3 leverRotationOffset = new Vector3(0f, 0f, -40f);

    [Header("Door Settings")]
    public string doorTag = "BarrierA";
    public float doorMoveDistance = -0.5f;
    public float doorMoveSpeed = 1.5f;

    private GameObject[] doors;
    private bool activated = false;

    private void Start()
    {
        // Find all doors with the tag
        doors = GameObject.FindGameObjectsWithTag(doorTag);

        Debug.Log("Doors Found: " + doors.Length);
    }

    private void OnTriggerStay(Collider other)
    {
        if (activated) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                activated = true;
                StartCoroutine(FlickThenOpenDoors());
            }
        }
    }

    private IEnumerator FlickThenOpenDoors()
    {
        yield return StartCoroutine(AnimateLever());

        bool doorsMoving = true;

        while (doorsMoving)
        {
            doorsMoving = false;

            foreach (GameObject door in doors)
            {
                Vector3 targetPosition = door.transform.position + new Vector3(0, 0, doorMoveDistance);

                if (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
                {
                    door.transform.position = Vector3.MoveTowards(
                        door.transform.position,
                        targetPosition,
                        doorMoveSpeed * Time.deltaTime
                    );

                    doorsMoving = true;
                }
            }

            yield return null;
        }
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        Vector3 endPos = startPos;
        Quaternion endRot = startRot * Quaternion.Euler(leverRotationOffset);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;
        transform.localRotation = endRot;
    }
}*/
/*

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
    private Vector3[] doorTargets;

    private bool activated = false;

    private void Start()
    {
        // Find all objects with the tag
        doors = GameObject.FindGameObjectsWithTag(doorTag);

        Debug.Log("Doors Found: " + doors.Length);

        // Precompute the target positions
        doorTargets = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            doorTargets[i] = doors[i].transform.position + new Vector3(0, moveUpAmount, 0);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (activated) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                activated = true;
                StartCoroutine(FlickThenOpenDoors());
            }
        }
    }

    private IEnumerator FlickThenOpenDoors()
    {
        yield return StartCoroutine(AnimateLever());

        bool doorsMoving = true;

        while (doorsMoving)
        {
            doorsMoving = false;

            for (int i = 0; i < doors.Length; i++)
            {
                Transform door = doors[i].transform;

                if (Vector3.Distance(door.position, doorTargets[i]) > 0.01f)
                {
                    door.position = Vector3.MoveTowards(
                        door.position,
                        doorTargets[i],
                        doorMoveSpeed * Time.deltaTime
                    );

                    doorsMoving = true;
                }
            }

            yield return null;
        }
    }

    private IEnumerator AnimateLever()
    {
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        Vector3 endPos = startPos;
        Quaternion endRot = startRot * Quaternion.Euler(leverRotationOffset);

        while (elapsed < leverAnimationTime)
        {
            float t = elapsed / leverAnimationTime;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;
        transform.localRotation = endRot;
    }
}*/

/*
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

    private void Start()
    {
        doors = GameObject.FindGameObjectsWithTag(doorTag);

        Debug.Log("Doors Found: " + doors.Length);

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

        isUp = !isUp;
        isMoving = false;
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
}*/



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