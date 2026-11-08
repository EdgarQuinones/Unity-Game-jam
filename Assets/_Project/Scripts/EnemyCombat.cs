using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    // Components
    private EnemyAI ai;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        ai = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        if (ai.CurrentState == EnemyAI.EnemyState.Attack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime) return;
        if (ai.Target == null) return;

        nextAttackTime = Time.time + attackCooldown;

        Vector2 direction =
            ((Vector2)ai.Target.position - (Vector2)transform.position).normalized;

        Vector2 attackPosition =
            (Vector2)transform.position + direction * attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRadius,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}