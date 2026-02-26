using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

[DisallowMultipleComponent]
public class PushPuzzleManager : MonoBehaviour
{
    public enum TriggerDirection
    {
        AnyPartHasZLessOrEqual,    // counts if any part of block has z <= threshold (good when threshold is negative)
        AnyPartHasZGreaterOrEqual, // counts if any part of block has z >= threshold (good when threshold is positive)
        SpansThreshold             // counts only if the block spans the threshold (min <= thr <= max)
    }

    [Header("Blocks to watch (assign Collider components)")]
    public List<Collider> blockColliders = new List<Collider>();

    [Tooltip("Optional: auto-populate blocks from this parent (will clear list first if set).")]
    public Transform autoPopulateFromParent;

    [Header("Nav Link")]
    public NavMeshLink connect1;

    [Header("Threshold")]
    public float thresholdZ = -4.47f;

    [Header("Behavior")]
    public TriggerDirection triggerMode = TriggerDirection.AnyPartHasZLessOrEqual;
    public bool allowDeactivate = false;
    public bool oneShot = true;
    public bool debugLogging = true;

    bool activated = false;

    void Start()
    {
        // Auto populate if requested
        if (autoPopulateFromParent != null)
        {
            blockColliders.Clear();
            foreach (var col in autoPopulateFromParent.GetComponentsInChildren<Collider>())
                blockColliders.Add(col);
        }

        if (connect1 == null)
        {
            Debug.LogWarning("PushPuzzleManager: connect1 (NavMeshLink) is not assigned.", this);
        }
        else
        {
            // Start disabled so we can enable when puzzle solved
            connect1.enabled = false;
        }

        if (blockColliders.Count == 0)
            Debug.LogWarning("PushPuzzleManager: blockColliders list is empty. Assign the block Collider components.", this);
    }

    void Update()
    {
        if (connect1 == null) return;
        if (oneShot && activated) return;

        bool allInPlace = true;

        for (int i = 0; i < blockColliders.Count; ++i)
        {
            Collider c = blockColliders[i];
            if (c == null)
            {
                allInPlace = false;
                if (debugLogging) Debug.Log($"PushPuzzleManager: block {i} is null -> not in place");
                break;
            }

            Bounds b = c.bounds;
            float minZ = b.min.z;
            float maxZ = b.max.z;

            bool counts = false;
            switch (triggerMode)
            {
                case TriggerDirection.AnyPartHasZLessOrEqual:
                    counts = (minZ <= thresholdZ);
                    break;
                case TriggerDirection.AnyPartHasZGreaterOrEqual:
                    counts = (maxZ >= thresholdZ);
                    break;
                case TriggerDirection.SpansThreshold:
                    counts = (minZ <= thresholdZ && maxZ >= thresholdZ);
                    break;
            }

            if (debugLogging)
            {
                Debug.Log($"PushPuzzleManager: Block[{i}] minZ={minZ:F3} maxZ={maxZ:F3} threshold={thresholdZ:F3} counts={counts}");
            }

            if (!counts)
            {
                allInPlace = false;
                break;
            }
        }

        if (allInPlace)
        {
            if (!connect1.enabled)
            {
                connect1.enabled = true;
                activated = true;
                Debug.Log("PushPuzzleManager: All blocks in place -> NavMeshLink activated");
            }
        }
        else
        {
            if (allowDeactivate && connect1.enabled)
            {
                connect1.enabled = false;
                activated = false;
                if (debugLogging) Debug.Log("PushPuzzleManager: A block left -> NavMeshLink deactivated");
            }
        }
    }
}