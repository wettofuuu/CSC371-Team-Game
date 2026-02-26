using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PushZone_More : MonoBehaviour
{
    [Tooltip("How close to spin time counts as a spin (seconds).")]
    public float spinWindow = 0.25f;

    private SpinToPush_More blockScript;

    void Awake()
    {
        // expects the push zone to be a child of the block
        blockScript = GetComponentInParent<SpinToPush_More>();
        if (blockScript == null)
            Debug.LogWarning($"PushZone_More on '{name}' couldn't find SpinToPush_More in parents.");
    }

    void OnTriggerStay(Collider other)
    {
        if (blockScript == null) return;

        // look for player's Movement component in parent chain
        Movement player = other.GetComponentInParent<Movement>();
        if (player == null) return;

        // only trigger a push if the player has recently spun
        if (Time.time - player.LastSpinTime > spinWindow) return;

        // hand off to block (it will de-duplicate repeated pushes using lastConsumedSpinTime)
        blockScript.TryPushFromPlayer(other, player.LastSpinTime);
    }
}