using System.Collections;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [Header("Lever References")]
    [SerializeField] private Transform leverHandle;

    [Header("Lever Pose")]
    [SerializeField] private bool useLocalRotation = true;
    [SerializeField] private Vector3 offEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 onEuler = new Vector3(0f, 0f, -40f);
    [SerializeField] private float leverMoveTime = 0.2f;

    [Header("Trigger Settings")]
    [SerializeField] private float retriggerCooldown = 0.4f;

    [Header("Cannons")]
    [SerializeField] private CannonShooter[] cannons;

    private bool isOn = false;
    private float nextTriggerTime = -999f;

    private Quaternion offRotation;
    private Quaternion onRotation;

    private void Start()
    {
        if (leverHandle == null)
            leverHandle = transform;

        offRotation = Quaternion.Euler(offEuler);
        onRotation = Quaternion.Euler(onEuler);

        if (useLocalRotation)
            leverHandle.localRotation = offRotation;
        else
            leverHandle.rotation = offRotation;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time < nextTriggerTime) return;

        Movement movement = other.GetComponent<Movement>();
        if (movement == null) return;

        if (!movement.IsSpinning) return;

        ToggleLever();
    }

    private void ToggleLever()
    {
        isOn = !isOn;
        nextTriggerTime = Time.time + retriggerCooldown;

        StopAllCoroutines();
        StartCoroutine(AnimateLever(isOn));

        if (cannons == null || cannons.Length == 0) return;

        foreach (CannonShooter cannon in cannons)
        {
            if (cannon == null) continue;

            if (isOn)
                cannon.StartContinuousFire();
            else
                cannon.StopContinuousFire();
        }
    }

    private IEnumerator AnimateLever(bool turnOn)
    {
        Quaternion from = useLocalRotation ? leverHandle.localRotation : leverHandle.rotation;
        Quaternion to = turnOn ? onRotation : offRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, leverMoveTime);

            if (useLocalRotation)
                leverHandle.localRotation = Quaternion.Slerp(from, to, t);
            else
                leverHandle.rotation = Quaternion.Slerp(from, to, t);

            yield return null;
        }

        if (useLocalRotation)
            leverHandle.localRotation = to;
        else
            leverHandle.rotation = to;
    }
}