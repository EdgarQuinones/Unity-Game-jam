using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    // Light Attack
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float lightCD = 1.5f;
    private float nextAttackTime = 0f;

    // Movement
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + lightCD;

        Attack();
    }

    private void Attack()
    {
        Vector2 direction = movement.FacingDirection;

        Vector2 attackPosition = (Vector2)transform.position + direction * attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(10f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        Vector2 direction = playerMovement != null
            ? playerMovement.FacingDirection
            : Vector2.down;

        Vector2 attackPosition =
            (Vector2)transform.position + direction * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRadius);
    }

}
