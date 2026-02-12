using System.Collections;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Snap-to transform (local by default)")]
    public bool useLocalTransform = true;

    public Vector3 snappedPosition = new Vector3(-0.5f, -0.15f, 0f);
    public Vector3 snappedRotationEuler = new Vector3(0f, 0f, -40f);

    public GameObject Portal;

    [Header("Scene transition")]
    public string level2SceneName = "Level2Transition";
    public float afterFlickDelay = 0.15f;

    private bool activated = false;

    private void OnTriggerStay(Collider other)
    {
        if (activated) return;

        if (other.TryGetComponent<Movement>(out var movement))
        {
            if (movement.IsSpinning)
            {
                Portal.SetActive(true);
                activated = true;
                StartCoroutine(FlickThenTransition());
            }
        }
    }

    private IEnumerator FlickThenTransition()
    {
        if (useLocalTransform)
        {
            transform.localPosition = snappedPosition;
            transform.localRotation = Quaternion.Euler(snappedRotationEuler);
        }
        else
        {
            transform.position = snappedPosition;
            transform.rotation = Quaternion.Euler(snappedRotationEuler);
        }

        yield return new WaitForSeconds(afterFlickDelay);

        // Fade + load next scene
    }
}
