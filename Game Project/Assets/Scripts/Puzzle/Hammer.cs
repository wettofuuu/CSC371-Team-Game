using UnityEngine;

public class HammerSwing : MonoBehaviour
{
    public float MaxAngle = 60f;
    public float Speed = 2f;

    void Update()
    {
        float angle = MaxAngle * Mathf.Sin(Time.time * Speed);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}