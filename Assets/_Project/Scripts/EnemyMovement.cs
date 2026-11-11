using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    // Components
    private Rigidbody2D rb;
    private EnemyAI ai;
    private Health health;
    private float speedMultiplier = 1f;
    private float speedBoostEndTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<EnemyAI>();
        health = GetComponent<Health>();
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (multiplier <= 1f || duration <= 0f)
            return;

        speedMultiplier = multiplier;
        speedBoostEndTime = Time.time + duration;
        if (health != null)
            health.SetSpeedBoostVisual(true);

        //Debug.Log($"{name}: speed boosted to {multiplier}x for {duration} seconds.", this);
    }

    private void Update()
    {
        if (speedMultiplier > 1f && Time.time >= speedBoostEndTime)
            ClearSpeedBoost();
    }

    private void OnDisable()
    {
        ClearSpeedBoost();
    }

    private void ClearSpeedBoost()
    {
        speedMultiplier = 1f;
        speedBoostEndTime = 0f;
        if (health != null)
            health.SetSpeedBoostVisual(false);
    }

    private void FixedUpdate()
    {

        if (ai.CurrentState != EnemyAI.EnemyState.Chase)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (ai.CurrentState)
        {
            case EnemyAI.EnemyState.Chase:
                Chase();
                break;

            case EnemyAI.EnemyState.Attack:
                Stop();
                break;
            case EnemyAI.EnemyState.Stunned:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    private void Chase()
    {
        Vector2 direction =
            ((Vector2)ai.Target.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed * speedMultiplier;
    }

    private void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}