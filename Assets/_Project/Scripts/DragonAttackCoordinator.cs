using UnityEngine;

[RequireComponent(typeof(DragonCharge), typeof(DragonFireBreath), typeof(EnemyAI))]
public class DragonAttackCoordinator : MonoBehaviour
{
    private DragonCharge charge;
    private DragonFireBreath breath;
    private EnemyAI ai;
    private bool chargeTurn;
    private bool attackStarted;

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
        charge.enabled = true;
    }

    private void Update()
    {
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

        chargeTurn = !chargeTurn;
        attackStarted = false;
        if (chargeTurn) charge.enabled = true;
        else breath.enabled = true;
    }

    private void OnDisable()
    {
        if (charge != null) charge.enabled = false;
        if (breath != null) breath.enabled = false;
    }
}