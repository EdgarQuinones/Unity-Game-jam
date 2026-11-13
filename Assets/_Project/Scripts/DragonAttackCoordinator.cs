using UnityEngine;

[RequireComponent(typeof(DragonCharge), typeof(DragonFireBreath), typeof(EnemyAI))]
public class DragonAttackCoordinator : MonoBehaviour
{
    private DragonCharge charge;
    private DragonFireBreath breath;
    private EnemyAI ai;
    private bool chargeTurn;
    private bool attackStarted;
    private int consecutiveAttacks;
    private float speedBoostEndTime;
    public float AttackSpeedMultiplier { get; private set; } = 1f;

    public void ApplyAttackSpeedBoost(float multiplier, float duration)
    {
        if (multiplier <= 1f || duration <= 0f)
            return;

        SetAttackSpeedMultiplier(multiplier);
        speedBoostEndTime = Time.time + duration;
        Debug.Log($"{name}: boss attack speed boosted to {multiplier}x for {duration} seconds (refreshed).", this);
    }

    private void SetAttackSpeedMultiplier(float multiplier)
    {
        float previous = AttackSpeedMultiplier;
        AttackSpeedMultiplier = multiplier;
        charge.AdjustWaitingTime(previous, multiplier);
        breath.AdjustWaitingTime(previous, multiplier);
    }

    private void Awake()
    {
        charge = GetComponent<DragonCharge>();
        breath = GetComponent<DragonFireBreath>();
        ai = GetComponent<EnemyAI>();
        charge.enabled = false;
        breath.enabled = false;
    }

    private void OnEnable()
    {
        chargeTurn = true;
        attackStarted = false;
        consecutiveAttacks = 0;
        charge.enabled = true;
    }

    private void Update()
    {
        if (AttackSpeedMultiplier > 1f && Time.time >= speedBoostEndTime)
        {
            SetAttackSpeedMultiplier(1f);
            Debug.Log($"{name}: boss attack speed boost expired.", this);
        }

        bool active = chargeTurn ? charge.IsAttacking : breath.IsAttacking;
        if (active)
        {
            attackStarted = true;
            return;
        }

        if (!attackStarted || ai.CurrentState == EnemyAI.EnemyState.Stunned)
            return;

        // Keep the charge component alive until it has restored player collision.
        if (chargeTurn && charge.IsRestoringPlayerCollision)
            return;

        if (chargeTurn) charge.enabled = false;
        else breath.enabled = false;

        consecutiveAttacks++;
        bool nextCharge = consecutiveAttacks >= 2 ? !chargeTurn : Random.value < 0.5f;
        if (nextCharge != chargeTurn)
            consecutiveAttacks = 0;
        chargeTurn = nextCharge;
        attackStarted = false;
        if (chargeTurn) charge.enabled = true;
        else breath.enabled = true;
    }

    private void OnDisable()
    {
        AttackSpeedMultiplier = 1f;
        speedBoostEndTime = 0f;
        if (charge != null) charge.enabled = false;
        if (breath != null) breath.enabled = false;
    }
}