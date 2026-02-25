using System.Collections;
using UnityEngine;

public class CameraToggleBirdseye : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform cameraTransform;
    public CameraZoneSwitchSimple zoneSwitch; // drag object that has CameraZoneSwitchSimple

    [Header("UI")]
    public GameObject toggleButtonUI; // button root to show/hide

    [Header("Show Button Condition")]
    public float xThreshold = -27f; // show when player.x < this
    public float zThreshold = -8f;  // show when player.z > this

    [Header("Birdseye Target")]
    public Transform location8;
    public float moveSmoothTime = 0.25f;
    private Vector3 velPos = Vector3.zero;

    [Header("Birdseye Rotation (Top Down)")]
    [Range(0f, 360f)] public float yawFacing = 180f; // adjust in Inspector if needed
    public float rotLerpSpeed = 12f;

    [Header("Return Blend")]
    public float returnBlendTime = 0.2f; // smooth restore of rotation

    private bool birdseyeOn = false;

    // Saved state from before entering birdseye
    private Vector3 savedPos;
    private Quaternion savedRot;
    private bool hasSaved = false;

    private Coroutine returnRoutine;

    void Start()
    {
        if (toggleButtonUI != null)
            toggleButtonUI.SetActive(false);
    }

    void Update()
    {
        if (player == null || cameraTransform == null) return;

        bool allowed = (player.position.x < xThreshold) && (player.position.z > zThreshold);

        if (toggleButtonUI != null && toggleButtonUI.activeSelf != allowed)
            toggleButtonUI.SetActive(allowed);

        // If we leave the allowed region, force birdseye OFF
        if (!allowed && birdseyeOn)
            SetBirdseye(false);

        if (birdseyeOn && location8 != null)
        {
            // override normal cam
            if (zoneSwitch != null) zoneSwitch.enabled = false;

            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                location8.position,
                ref velPos,
                moveSmoothTime
            );

            // true top-down
            Quaternion targetRot = Quaternion.Euler(90f, yawFacing, 0f);
            cameraTransform.rotation = Quaternion.Slerp(
                cameraTransform.rotation,
                targetRot,
                Time.deltaTime * rotLerpSpeed
            );
        }
    }

    // Hook this to your UI Button OnClick()
    public void ToggleBirdseye()
    {
        bool allowed = (player.position.x < xThreshold) && (player.position.z > zThreshold);
        if (!allowed) return;

        SetBirdseye(!birdseyeOn);
    }

    private void SetBirdseye(bool on)
    {
        // stop any return blend currently running
        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }

        if (on)
        {
            // Save current camera state so we can restore it cleanly
            savedPos = cameraTransform.position;
            savedRot = cameraTransform.rotation;
            hasSaved = true;

            birdseyeOn = true;
            velPos = Vector3.zero;

            if (zoneSwitch != null) zoneSwitch.enabled = false;
        }
        else
        {
            birdseyeOn = false;
            velPos = Vector3.zero;

            // Restore rotation/position smoothly, THEN re-enable zone switching
            if (hasSaved)
                returnRoutine = StartCoroutine(ReturnToSavedState());
            else
                if (zoneSwitch != null) zoneSwitch.enabled = true;
        }
    }

    private IEnumerator ReturnToSavedState()
    {
        // keep zone switch OFF during the blend so it doesn't fight
        if (zoneSwitch != null) zoneSwitch.enabled = false;

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        float t = 0f;
        float dur = Mathf.Max(0.01f, returnBlendTime);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;

            cameraTransform.position = Vector3.Lerp(startPos, savedPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, savedRot, t);

            yield return null;
        }

        cameraTransform.position = savedPos;
        cameraTransform.rotation = savedRot;

        // now let normal zone switching take back over (position changes)
        if (zoneSwitch != null) zoneSwitch.enabled = true;

        returnRoutine = null;
    }
}