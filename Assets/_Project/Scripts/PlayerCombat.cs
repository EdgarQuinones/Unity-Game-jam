using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Light Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float lightCD = 1.5f;

    [Header("Enemy Detection")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private float stunCooldown = 5f;
    [SerializeField] private float stunRadius = 1.5f;

    [Header("Sealing")]
    [SerializeField] private float sealRange = 1f;
    [SerializeField] private float sealRadius = 0.5f;

    private PlayerMovement movement;

    private float nextAttackTime = 0f;
    private float nextStunTime = 0f;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    // NORMAL ATTACK
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + lightCD;

        Attack();
    }

    private void Attack()
    {
        Vector2 direction = movement.FacingDirection;

        Vector2 attackPosition =
            (Vector2)transform.position +
            direction * attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponentInParent<Health>();

            if (enemyHealth != null)
                enemyHealth.TakeDamage(attackDamage);
        }
    }

    // E BUTTON - SEAL OR STUN
    public void OnStun(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // Sealing takes priority over stunning
        if (TrySealEnemy())
            return;

        // Only stun uses a cooldown
        if (Time.time < nextStunTime)
            return;

        nextStunTime = Time.time + stunCooldown;

        StunNearbyEnemies();
    }

    private bool TrySealEnemy()
    {
        Vector2 direction = movement.FacingDirection;

        Vector2 sealPosition =
            (Vector2)transform.position +
            direction * sealRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            sealPosition,
            sealRadius,
            enemyLayer
        );

        EnemyAI closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            EnemyAI enemy = hit.GetComponentInParent<EnemyAI>();

            if (enemy == null || !enemy.CanBeSealed)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
            return false;

        closestEnemy.Seal();
        return true;
    }

    private void StunNearbyEnemies()
    {
        Vector2 stunPosition = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            stunPosition,
            stunRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyAI enemy = hit.GetComponentInParent<EnemyAI>();

            if (enemy != null)
            {
                enemy.Stun(
                    stunDuration,
                    EnemyAI.StunType.Player
                );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        PlayerMovement playerMovement =
            GetComponent<PlayerMovement>();

        Vector2 direction = playerMovement != null
            ? playerMovement.FacingDirection
            : Vector2.down;

        // Normal attack - Red
        Vector2 attackPosition =
            (Vector2)transform.position +
            direction * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRadius);

        // AOE Stun - Cyan
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stunRadius);

        // Sealing area - Green
        Vector2 sealPosition =
            (Vector2)transform.position +
            direction * sealRange;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(sealPosition, sealRadius);
    }
}