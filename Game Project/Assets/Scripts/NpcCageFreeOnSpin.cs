using UnityEngine;
using System.Collections;

public class NpcCageFreeOnSpin : MonoBehaviour
{
    [Header("Spin Requirements")]
    [SerializeField] private float spinWindow = 0.25f;

    [Header("What happens when freed")]
    [SerializeField] private GameObject cageObject;       // optional: hide whole cage
    [SerializeField] private GameObject portalExitLevel3; // assign your hidden portal

    [Header("Cage Door Animation")]
    [SerializeField] private Transform cageDoor;          // drag cageDoor here
    [SerializeField] private float doorDropDistance = 2f; // how far down
    [SerializeField] private float doorDropTime = 0.5f;   // how fast
    [SerializeField] private float doorDisableDelay = 0.1f; // after it lands
    [SerializeField] private bool disableDoorAfterDrop = true;

    [SerializeField] private bool disableThisNpcWhenFreed = true;

    private bool freed = false;
    private float lastConsumedSpinTime = -999f;

    private void Awake()
    {
        if (portalExitLevel3 != null)
            portalExitLevel3.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (freed) return;

        Movement movement = other.GetComponentInParent<Movement>();
        if (movement == null) return;

        // Must have key first
        if (!KeyState.HasKey) return;

        // Must have spun recently
        float spinTime = movement.LastSpinTime;
        if ((Time.time - spinTime) > spinWindow) return;

        // One trigger per spin
        if (spinTime <= lastConsumedSpinTime) return;
        lastConsumedSpinTime = spinTime;

        FreeNpc();
    }

    private void FreeNpc()
    {
        freed = true;

        // reveal portal
        if (portalExitLevel3 != null)
            portalExitLevel3.SetActive(true);

        // animate door drop (instead of instantly hiding cage)
        if (cageDoor != null)
            StartCoroutine(DropDoorThenDisable());
        else
        {
            // fallback: if no door assigned, you can still hide cage
            if (cageObject != null) cageObject.SetActive(false);
        }

        if (disableThisNpcWhenFreed)
            gameObject.SetActive(false);
    }

    private IEnumerator DropDoorThenDisable()
    {
        Vector3 start = cageDoor.position;
        Vector3 end = start + Vector3.down * doorDropDistance;

        float t = 0f;
        float dur = Mathf.Max(0.01f, doorDropTime);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            cageDoor.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        cageDoor.position = end;

        if (doorDisableDelay > 0f)
            yield return new WaitForSeconds(doorDisableDelay);

        if (disableDoorAfterDrop)
            cageDoor.gameObject.SetActive(false);

        // optional: if you want the rest of the cage to disappear too:
        // if (cageObject != null) cageObject.SetActive(false);
    }
}