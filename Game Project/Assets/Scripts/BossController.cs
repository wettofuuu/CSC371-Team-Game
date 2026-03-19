using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Movement playerMovement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider bossCapsule;

    [Header("Boss Zones")]
    [SerializeField] private Collider attackZoneCollider;
    [SerializeField] private Collider followZoneCollider;

    [Header("Boss Model / Visuals")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Renderer[] bossRenderers;
    [SerializeField] private GameObject attackIndicator;

    [Header("Follow")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float stopDistance = 2.1f;
    [SerializeField] private float extraSeparation = 0.25f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2.4f;
    [SerializeField] private float attackWindup = 0.8f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackSpinSpeed = 950f;
    [SerializeField] private float telegraphSpinSpeed = 180f;

    [Header("Taking Damage")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private float playerHitDistance = 2.2f;
    [SerializeField] private float hurtFlashDuration = 0.12f;

    [Header("Level Complete")]
    [SerializeField] private string nextSceneName = "WinScene";
    [SerializeField] private float popupDelay = 2f;
    [SerializeField] private Vector3 deathEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private LevelCompletePopup popup;

    [Header("Boss UI")]
    [SerializeField] private BossHealthUI bossHealthUI;

    private int currentHealth;
    private bool playerInFollowZone;
    private bool playerInAttackZone;
    private bool isAttacking;
    private bool canDealDamage;
    private bool isDead;
    private bool hasDamagedPlayerThisAttack;
    private float lastAttackTime = -999f;
    private float lastRegisteredPlayerSpinTime = -999f;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (bossHealthUI != null)
        {
            bossHealthUI.SetHealth(currentHealth, maxHealth);
            bossHealthUI.Hide();
        }

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (bossCapsule == null)
            bossCapsule = GetComponent<CapsuleCollider>();

        if (visualRoot == null)
            visualRoot = transform;

        if (attackIndicator != null)
            attackIndicator.SetActive(false);

        mpb = new MaterialPropertyBlock();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<Movement>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerMovement = p.GetComponent<Movement>();
            }
            return;
        }

        CheckForPlayerSpinDamage();

        if (!isAttacking && playerInAttackZone && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        if (!isAttacking && playerInFollowZone)
        {
            FollowPlayerButKeepDistance();
        }
    }

    private void FollowPlayerButKeepDistance()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude < 0.0001f)
            return;

        Vector3 dir = toPlayer.normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        float desiredStop = GetDesiredStopDistance();
        float dist = toPlayer.magnitude;

        if (dist <= desiredStop)
            return;

        float moveAmount = Mathf.Min(moveSpeed * Time.deltaTime, dist - desiredStop);
        transform.position += dir * moveAmount;
    }

    private float GetDesiredStopDistance()
    {
        float bossRadius = 0.5f;
        if (bossCapsule != null)
            bossRadius = Mathf.Max(bossCapsule.radius * transform.lossyScale.x, bossCapsule.radius * transform.lossyScale.z);

        float playerRadius = 0.5f;
        CapsuleCollider playerCapsule = player != null ? player.GetComponent<CapsuleCollider>() : null;
        if (playerCapsule != null)
            playerRadius = Mathf.Max(playerCapsule.radius * player.lossyScale.x, playerCapsule.radius * player.lossyScale.z);

        return Mathf.Max(stopDistance, bossRadius + playerRadius + extraSeparation);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canDealDamage = false;
        hasDamagedPlayerThisAttack = false;
        lastAttackTime = Time.time;

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look.normalized);

        if (attackIndicator != null)
            attackIndicator.SetActive(true);

        float windupTimer = 0f;
        while (windupTimer < attackWindup)
        {
            windupTimer += Time.deltaTime;

            if (visualRoot != null)
                visualRoot.Rotate(Vector3.up, telegraphSpinSpeed * Time.deltaTime, Space.Self);

            yield return null;
        }

        if (attackIndicator != null)
            attackIndicator.SetActive(false);

        if (!playerInAttackZone || isDead)
        {
            isAttacking = false;
            yield break;
        }

        canDealDamage = true;

        float attackTimer = 0f;
        while (attackTimer < attackDuration)
        {
            attackTimer += Time.deltaTime;

            if (visualRoot != null)
                visualRoot.Rotate(Vector3.up, attackSpinSpeed * Time.deltaTime, Space.Self);

            yield return null;
        }

        canDealDamage = false;
        isAttacking = false;
    }

    private void CheckForPlayerSpinDamage()
    {
        if (playerMovement == null || isDead) return;
        if (!playerMovement.IsSpinning) return;

        float spinTime = playerMovement.LastSpinTime;
        if (Mathf.Approximately(spinTime, lastRegisteredPlayerSpinTime))
            return;

        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = 0f;
        b.y = 0f;

        float dist = Vector3.Distance(a, b);
        if (dist <= playerHitDistance)
        {
            lastRegisteredPlayerSpinTime = spinTime;
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Boss took damage. Current health: " + currentHealth);

        if (bossHealthUI != null)
            bossHealthUI.SetHealth(currentHealth, maxHealth);

        StartCoroutine(HurtFlash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator HurtFlash()
    {
        SetBossEmission(3.5f);
        yield return new WaitForSeconds(hurtFlashDuration);
        SetBossEmission(0f);
    }

    private void SetBossEmission(float intensity)
    {
        if (bossRenderers == null) return;

        foreach (Renderer r in bossRenderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", Color.red * intensity);
            r.SetPropertyBlock(mpb);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        canDealDamage = false;
        isAttacking = false;
        playerInAttackZone = false;
        playerInFollowZone = false;

        if (attackIndicator != null)
            attackIndicator.SetActive(false);

        if (bossHealthUI != null)
            bossHealthUI.Hide();

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(deathEuler);

        float t = 0f;
        float duration = 0.6f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(popupDelay);

        if (popup != null)
        {
            popup.SetNextScene(nextSceneName);
            popup.Show();
        }
        else
        {
            Debug.LogWarning("LevelCompletePopup is not assigned on BossController.");
        }
    }

    public void SetPlayerInFollowZone(bool value)
    {
        if (isDead) return;

        playerInFollowZone = value;

        if (bossHealthUI != null)
        {
            if (value) bossHealthUI.Show();
            else bossHealthUI.Hide();
        }
    }

    public void SetPlayerInAttackZone(bool value)
    {
        if (isDead) return;
        playerInAttackZone = value;
    }

    public bool CanDealDamage()
    {
        return canDealDamage && !isDead && !hasDamagedPlayerThisAttack;
    }

    public void DamagePlayer(GameObject other)
    {
        if (!CanDealDamage()) return;
        if (!other.CompareTag("Player")) return;

        Movement movement = other.GetComponent<Movement>();
        if (movement != null)
        {
            hasDamagedPlayerThisAttack = true;
            movement.TakeLifeDamage(1);
        }
    }
}