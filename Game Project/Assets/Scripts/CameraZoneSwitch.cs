using UnityEngine;

public class CameraZoneSwitch : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;

    [Header("Trigger Conditions")]
    public float triggerX = -6.7f;
    public float triggerZ = 2.9f;

    [Header("Camera Target Position")]
    public Vector3 newCameraPosition;

    [Header("Swoop Settings")]
    public float smoothTime = 0.7f;

    private bool swooping = false;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Start swoop once
        if (!swooping &&
            player.position.x < triggerX &&
            player.position.z < triggerZ)
        {
            Debug.Log("Hello");
            swooping = true;
        }

        if (swooping)
        {
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                newCameraPosition,
                ref velocity,
                smoothTime
            );

            // Stop when close enough
            if (Vector3.Distance(cameraTransform.position, newCameraPosition) < 0.05f)
            {
                cameraTransform.position = newCameraPosition;
                swooping = false;
            }
        }
    }
}
