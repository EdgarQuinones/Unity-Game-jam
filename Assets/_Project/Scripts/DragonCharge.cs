using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyAI))]
public class DragonCharge : MonoBehaviour
{
    private enum ChargeState { Waiting, Warning, Charging, Recovery }

    [Header("Charge Timing")]
    [SerializeField] private float timeBetweenCharges = 4f;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float recoveryDuration = 1f;

    [Header("Charge Movement")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeDistance = 6f;
    [SerializeField] private float chargeDamage = 20f;
    [SerializeField] private float playerStunDuration = 0.75f;
    [SerializeField] private float pillarStunDuration = 5f;

    [Header("Warning")]
    [SerializeField] private LineRenderer warningLine;

    private Rigidbody2D body;
    private EnemyAI ai;
    private EnemyMovement movement;
    private EnemyCombat combat;
    private ChargeState state;
    private float stateEndTime;
    private Vector2 chargeDirection;
    private Vector2 chargeStart;
    private bool movementWasEnabled;
    private bool combatWasEnabled;
    private bool controlsTaken;
    private Collider2D bossCollider;
    private Collider2D[] playerColliders;
    private bool playerCollisionIgnored;
    private bool playerHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ai = GetComponent<EnemyAI>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        bossCollider = GetComponent<Collider2D>();

        if (warningLine != null)
            warningLine.enabled = false;
    }

    private void OnEnable()
    {
        state = ChargeState.Waiting;
        stateEndTime = Time.time + Mathf.Max(0f, timeBetweenCharges);
    }

    private void Update()
    {
        if (playerCollisionIgnored && state != ChargeState.Charging)
            RestorePlayerCollisionWhenSeparated();

        if (ai.CurrentState == EnemyAI.EnemyState.Stunned || ai.Target == null || !ai.Target.gameObject.activeInHierarchy)
        {
            if (controlsTaken)
                CancelCharge();
            stateEndTime = Time.time + Mathf.Max(0f, timeBetweenCharges);
            return;
        }

        if (state == ChargeState.Waiting && Time.time >= stateEndTime)
            BeginWarning();
        else if (state == ChargeState.Warning && Time.time >= stateEndTime)
            BeginCharge();
        else if (state == ChargeState.Recovery && Time.time >= stateEndTime)
            FinishCharge();
    }

    private void FixedUpdate()
    {
        if (state != ChargeState.Charging || ai.CurrentState == EnemyAI.EnemyState.Stunned)
            return;

        float travelled = Vector2.Dot(body.position - chargeStart, chargeDirection);
        if (travelled >= chargeDistance || Time.time >= stateEndTime)
        {
            BeginRecovery();
            return;
        }

        float step = Mathf.Min(chargeSpeed * Time.fixedDeltaTime, chargeDistance - travelled);
        CheckPlayerHit(step);
        body.linearVelocity = chargeDirection * (step / Time.fixedDeltaTime);
    }

    private void BeginWarning()
    {
        if (warningLine == null || chargeSpeed <= 0f || chargeDistance <= 0f)
        {
            Debug.LogWarning($"{name}: Dragon Charge needs a warning line and positive speed and distance.", this);
            stateEndTime = Time.time + Mathf.Max(1f, timeBetweenCharges);
            return;
        }

        Vector2 direction = (Vector2)ai.Target.position - body.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            stateEndTime = Time.time + 1f;
            return;
        }

        chargeDirection = direction.normalized;
        movementWasEnabled = movement != null && movement.enabled;
        combatWasEnabled = combat != null && combat.enabled;
        controlsTaken = true;
        if (movementWasEnabled) movement.enabled = false;
        if (combatWasEnabled) combat.enabled = false;
        body.linearVelocity = Vector2.zero;

        warningLine.useWorldSpace = true;
        warningLine.positionCount = 2;
        Vector3 start = transform.position;
        warningLine.SetPosition(0, start);
        warningLine.SetPosition(1, start + (Vector3)(chargeDirection * chargeDistance));
        warningLine.enabled = true;
        state = ChargeState.Warning;
        stateEndTime = Time.time + Mathf.Max(0f, warningDuration);
        Debug.Log($"{name}: dragon charge warning.", this);
    }

    private void BeginCharge()
    {
        warningLine.enabled = false;
        chargeStart = body.position;
        playerHit = false;
        playerColliders = ai.Target.GetComponentsInChildren<Collider2D>();
        if (bossCollider != null)
        {
            foreach (Collider2D playerCollider in playerColliders)
                if (playerCollider != null && playerCollider.enabled && playerCollider.gameObject.activeInHierarchy)
                    Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
            playerCollisionIgnored = true;
        }
        state = ChargeState.Charging;
        stateEndTime = Time.time + chargeDistance / chargeSpeed;
        Debug.Log($"{name}: dragon charge started.", this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != ChargeState.Charging)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Pillar"))
        {
            body.linearVelocity = Vector2.zero;
            ai.Stun(pillarStunDuration, EnemyAI.StunType.Environmental);
            Debug.Log($"{name}: hit a pillar and was environmentally stunned for {pillarStunDuration} seconds.", this);
            CancelCharge();
        }
        else
        {
            BeginRecovery();
        }
    }

    private void CheckPlayerHit(float step)
    {
        if (playerHit || bossCollider == null || playerColliders == null)
            return;

        Bounds bounds = bossCollider.bounds;
        float radius = Mathf.Max(bounds.extents.x, bounds.extents.y);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(bounds.center, radius, chargeDirection, step);
        foreach (RaycastHit2D hit in hits)
        {
            foreach (Collider2D playerCollider in playerColliders)
            {
                if (hit.collider != playerCollider) continue;
                playerHit = true;
                Health playerHealth = ai.Target.GetComponentInParent<Health>();
                PlayerMovement playerMovement = ai.Target.GetComponentInParent<PlayerMovement>();
                if (playerHealth != null) playerHealth.TakeDamage(chargeDamage);
                if (playerMovement != null && playerMovement.gameObject.activeInHierarchy)
                    playerMovement.ApplyStun(playerStunDuration);
                Debug.Log($"{name}: charge hit player for {chargeDamage} damage.", this);
                return;
            }
        }
    }

    private void RestorePlayerCollisionWhenSeparated()
    {
        if (bossCollider == null || playerColliders == null)
        {
            playerCollisionIgnored = false;
            return;
        }

        foreach (Collider2D playerCollider in playerColliders)
            if (playerCollider != null && playerCollider.enabled && playerCollider.gameObject.activeInHierarchy &&
                bossCollider.bounds.Intersects(playerCollider.bounds))
                return;

        RestorePlayerCollision();
    }

    private void RestorePlayerCollision()
    {
        if (bossCollider != null && playerColliders != null && bossCollider.gameObject.activeInHierarchy)
            foreach (Collider2D playerCollider in playerColliders)
                if (playerCollider != null && playerCollider.enabled && playerCollider.gameObject.activeInHierarchy)
                    Physics2D.IgnoreCollision(bossCollider, playerCollider, false);
        playerCollisionIgnored = false;
    }

    private void BeginRecovery()
    {
        body.linearVelocity = Vector2.zero;
        state = ChargeState.Recovery;
        stateEndTime = Time.time + Mathf.Max(0f, recoveryDuration);
        Debug.Log($"{name}: dragon charge recovery.", this);
    }

    private void FinishCharge()
    {
        ReleaseControls();
        state = ChargeState.Waiting;
        stateEndTime = Time.time + Mathf.Max(0f, timeBetweenCharges);
    }

    private void CancelCharge()
    {
        ReleaseControls();
        state = ChargeState.Waiting;
        Debug.Log($"{name}: dragon charge interrupted.", this);
    }

    private void ReleaseControls()
    {
        if (warningLine != null) warningLine.enabled = false;
        if (!controlsTaken) return;

        body.linearVelocity = Vector2.zero;
        if (movement != null) movement.enabled = movementWasEnabled;
        if (combat != null) combat.enabled = combatWasEnabled;
        controlsTaken = false;
    }

    private void OnDisable()
    {
        ReleaseControls();
        RestorePlayerCollision();
        state = ChargeState.Waiting;
    }
}