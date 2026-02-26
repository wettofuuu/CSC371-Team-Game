using UnityEngine;

public class CameraZoneSwitchSimple : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;

    [Header("Camera Targets")]
    public Transform location1;   // x in [-19, -5)
    public Transform location6;   // x in [-26, -19)
    public Transform location7;   // x < -26

    public Transform location2;   // z in [-17, -8.5)
    public Transform location3;   // z in [-26, -17)
    public Transform location4;   // z in [-32, -26)
    public Transform location5;   // z < -32
    public Transform defaultCam;  // optional

    [Header("Swoop Settings")]
    public float smoothTime = 0.7f;

    private Vector3 velocity = Vector3.zero;
    private Transform currentTarget;
    private bool swooping = false;

    void Start()
    {
        if (player == null || cameraTransform == null) return;

        currentTarget = GetTargetForPlayer(player.position);
        if (currentTarget != null)
            cameraTransform.position = currentTarget.position;
    }

    void Update()
    {
        if (player == null || cameraTransform == null) return;

        Transform target = GetTargetForPlayer(player.position);

        if (target != currentTarget)
        {
            currentTarget = target;
            swooping = true;
        }

        if (swooping && currentTarget != null)
        {
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                currentTarget.position,
                ref velocity,
                smoothTime
            );

            if (Vector3.Distance(cameraTransform.position, currentTarget.position) < 0.05f)
            {
                cameraTransform.position = currentTarget.position;
                velocity = Vector3.zero;
                swooping = false;
            }
        }
    }

    private Transform GetTargetForPlayer(Vector3 playerPos)
    {
        float z = playerPos.z;
        float x = playerPos.x;

        // Z-based zones (range-based, works both directions)
        if (z < -32f && location5 != null) return location5;
        if (z >= -32f && z < -26f && location4 != null) return location4;
        if (z >= -26f && z < -17f && location3 != null) return location3;
        if (z >= -17f && z < -8.5f && location2 != null) return location2;

        // X-based zones (only if not in any of the Z zones above)
        if (x < -26f && location7 != null) return location7;
        if (x >= -26f && x < -19f && location6 != null) return location6;
        if (x >= -19f && x < -5f && location1 != null) return location1;

        return defaultCam;
    }
}