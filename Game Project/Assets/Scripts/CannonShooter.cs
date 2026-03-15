using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("References")]
    [SerializeField] private Transform rotatingPart;   // drag Small_cannon_Aim here
    [SerializeField] private Transform firePoint;      // drag FirePoint here
    [SerializeField] private GameObject shellPrefab;   // drag Round_shot prefab here

    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private bool onlyShootInRange = true;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private bool onlyRotateOnY = true;

    [Header("Shooting")]
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float shellSpeed = 12f;
    [SerializeField] private float shellLifetime = 5f;

    private float fireTimer = 0f;

    private void Update()
    {
        if (target == null || rotatingPart == null || firePoint == null || shellPrefab == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (onlyShootInRange && distance > detectionRange)
            return;

        RotateTowardTarget();

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void RotateTowardTarget()
    {
        Vector3 direction = target.position - rotatingPart.position;

        if (onlyRotateOnY)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        rotatingPart.rotation = Quaternion.Slerp(
            rotatingPart.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    private void Shoot()
    {
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = shell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * shellSpeed;
        }

        Destroy(shell, shellLifetime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}
