using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    public enum ZoneType
    {
        Follow,
        Attack
    }

    [SerializeField] private BossController boss;
    [SerializeField] private ZoneType zoneType;

    private void Reset()
    {
        if (boss == null)
            boss = GetComponentInParent<BossController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.Follow)
            boss.SetPlayerInFollowZone(true);
        else
            boss.SetPlayerInAttackZone(true);

        if (zoneType == ZoneType.Attack)
            boss.DamagePlayer(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.Attack)
            boss.DamagePlayer(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.Follow)
            boss.SetPlayerInFollowZone(false);
        else
            boss.SetPlayerInAttackZone(false);
    }
}