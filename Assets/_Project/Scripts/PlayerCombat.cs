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

    // Components
    private PlayerMovement movement;
    private GameManager gameManager;

    // Cooldowns
    private float nextAttackTime = 0f;
    private float nextStunTime = 0f;

    public float AttackCooldownProgress => CooldownProgress(nextAttackTime, lightCD);
    public float StunCooldownProgress => CooldownProgress(nextStunTime, stunCooldown);

    // Presentation hooks fire only after an action passes its existing gameplay checks.
    public event System.Action AttackPerformed;
    public event System.Action StunPerformed;

    private static float CooldownProgress(float readyTime, float duration)
    {
        if (duration <= 0f)
            return 1f;

        return 1f - Mathf.Clamp01((readyTime - Time.time) / duration);
    }

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || movement.IsStunned || PauseManager.IsPaused)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + lightCD;

        Attack();
        AttackPerformed?.Invoke();
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
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // E BUTTON - SEAL OR STUN
    public void OnStun(InputAction.CallbackContext context)
    {
        if (!context.performed || movement.IsStunned || PauseManager.IsPaused)
            return;

        if (gameManager != null && gameManager.CurrentPhase == GameManager.GamePhase.Cleaning)
        {
            TalismanPuzzle puzzle = FindFirstObjectByType<TalismanPuzzle>();
            if (puzzle != null && puzzle.TryCleanseNearest(transform.position))
                return;

            LanternPuzzle lanternPuzzle = FindFirstObjectByType<LanternPuzzle>();
            if (lanternPuzzle != null)
                lanternPuzzle.TryLightNearest(transform.position);
            return;
        }

        // Sealing takes priority over stunning
        if (TrySealEnemy())
            return;

        // The player stun cannot affect the boss; keep it available during this phase.
        if (gameManager != null && gameManager.CurrentPhase == GameManager.GamePhase.Boss)
            return;

        // Check stun cooldown
        if (Time.time < nextStunTime)
            return;

        nextStunTime = Time.time + stunCooldown;

        StunNearbyEnemies();
        StunPerformed?.Invoke();
    }

    // SEALING
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

        Debug.Log($"Sealed {closestEnemy.gameObject.name}!");

        return true;
    }

    // AOE STUN
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

            if (enemy != null && !enemy.IsBoss)
            {
                enemy.Stun(
                    stunDuration,
                    EnemyAI.StunType.Player
                );

                Debug.Log($"Player stunned {enemy.gameObject.name}!");
            }
        }
    }

    // DEBUG GIZMOS
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

        // AOE stun - Cyan
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stunRadius);

        // Seal range - Green
        Vector2 sealPosition =
            (Vector2)transform.position +
            direction * sealRange;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(sealPosition, sealRadius);
    }
}