using UnityEngine;

public class CameraZoneSwitch : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;

    [Header("Camera Targets")]
    public Transform defaultCam;   // where camera goes when x >= -2
    public Transform secondCam;
    public Transform thirdCam;
    public Transform fourthCam;
    public Transform fifthCam;     // birdseye rotation
    public Transform sixthCam;     // NEW: x < -80

    [Header("Swoop Settings")]
    public float smoothTime = 0.7f;

    [Header("Rotation (FifthCam only)")]
    public float rotationSmoothSpeed = 5f;

    private Vector3 velocity = Vector3.zero;
    private Transform currentTarget;
    private bool swooping = false;

    private bool rotateToTarget = false;
    private Quaternion targetRotation;

    private Quaternion normalRotation;   // rotation to restore when leaving FifthCam
    private bool wasInFifth = false;

    void Start()
    {
        normalRotation = cameraTransform.rotation;

        currentTarget = GetTargetForX(player.position.x);
        if (currentTarget != null)
            cameraTransform.position = currentTarget.position;
    }

    void Update()
    {
        Transform target = GetTargetForX(player.position.x);

        if (target != currentTarget)
        {
            bool enteringFifth = (target == fifthCam) && !wasInFifth;
            bool leavingFifth  = (target != fifthCam) && wasInFifth; // includes going to SixthCam

            currentTarget = target;
            swooping = true;

            if (enteringFifth)
            {
                // Save the rotation we want to go back to later (isometric/normal)
                normalRotation = cameraTransform.rotation;

                rotateToTarget = true;
                targetRotation = Quaternion.LookRotation(Vector3.down, Vector3.left);
                wasInFifth = true;
            }
            else if (leavingFifth)
            {
                // Going to any non-fifth cam (including SixthCam): revert to normal/isometric
                rotateToTarget = true;
                targetRotation = normalRotation;
                wasInFifth = false;
            }
            else
            {
                // Any other switch not involving FifthCam: no rotation changes
                rotateToTarget = false;
                wasInFifth = (target == fifthCam);
            }
        }

        if (swooping && currentTarget != null)
        {
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                currentTarget.position,
                ref velocity,
                smoothTime
            );

            if (rotateToTarget)
            {
                cameraTransform.rotation = Quaternion.Slerp(
                    cameraTransform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSmoothSpeed
                );
            }

            bool closePos = Vector3.Distance(cameraTransform.position, currentTarget.position) < 0.05f;
            bool closeRot = !rotateToTarget || Quaternion.Angle(cameraTransform.rotation, targetRotation) < 0.5f;

            if (closePos && closeRot)
            {
                cameraTransform.position = currentTarget.position;
                if (rotateToTarget) cameraTransform.rotation = targetRotation;
                swooping = false;
            }
        }
    }

    private Transform GetTargetForX(float x)
    {
        if (x < -80f) return sixthCam;   // NEW
        if (x < -49f) return fifthCam;
        if (x < -43f) return fourthCam;
        if (x < -27f) return thirdCam;
        if (x < -2f)  return secondCam;
        return defaultCam;
    }
}