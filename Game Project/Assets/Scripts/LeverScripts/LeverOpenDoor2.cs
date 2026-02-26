using System.Collections;
using UnityEngine;

public class LeverOpenDoor2 : MonoBehaviour
{
    [Header("Snap-to transform (local by default)")]
    public bool useLocalTransform = true;

    public Vector3 snappedPosition = new Vector3(-0.5f, -0.15f, 0f);
    public Vector3 snappedRotationEuler = new Vector3(0f, 0f, -40f);

    [Header("Lever Animation")]
    public float leverAnimationTime = 0.25f;

   [Header("Door Settings")]
    public Transform door;
    public float targetX = 3f;   // set this in inspector
    public float doorMoveSpeed = 1.5f;

    [Header("Door Audio")]
    public AudioSource doorAudio;   // Drag the Door's AudioSource here

    private bool activated = false;

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

        // 🔊 PLAY DOOR SOUND ONCE WHEN DOOR STARTS MOVING
        if (doorAudio == null && door != null)
            doorAudio = door.GetComponent<AudioSource>();

        if (doorAudio != null)
            doorAudio.Play();

        // Move door smoothly
        if (door != null)
        {
            Vector3 targetPosition = new Vector3(
                targetX,
                door.position.y,
                door.position.z
            );

            while (Vector3.Distance(door.position, targetPosition) > 0.01f)
            {
                door.position = Vector3.MoveTowards(
                    door.position,
                    targetPosition,
                    doorMoveSpeed * Time.deltaTime
                );

                yield return null;
            }
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

        // Ensure final position is exact
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
}
