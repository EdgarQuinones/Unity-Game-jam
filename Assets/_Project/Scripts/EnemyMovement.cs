using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    // Components
    private Rigidbody2D rb;
    private EnemyAI ai;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<EnemyAI>();
    }

    private void FixedUpdate()
    {
        switch (ai.CurrentState)
        {
            case EnemyAI.EnemyState.Chase:
                Chase();
                break;

            case EnemyAI.EnemyState.Attack:
                Stop();
                break;
        }
    }

    private void Chase()
    {
        Vector2 direction =
            ((Vector2)ai.Target.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    private void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}