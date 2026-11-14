using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement), typeof(PlayerCombat), typeof(Health))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private enum VisualState { Idle, Attack, CastStun, Stunned, Hurt }

    [Header("Renderer")]
    [SerializeField] private SpriteRenderer playerRenderer;
    [Tooltip("Use the unflipped artwork for rightward facing.")]
    [SerializeField] private bool artworkFacesRight = true;

    [Header("Frames (in numbered order)")]
    [Tooltip("Static standing/moving fallback until idle and walking artwork is supplied.")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private Sprite[] hurtFrames;
    [SerializeField] private Sprite[] stunFrames;
    [SerializeField, Min(1f)] private float attackFramesPerSecond = 8f;
    [SerializeField, Min(1f)] private float hurtFramesPerSecond = 10f;
    [SerializeField, Min(1f)] private float stunFramesPerSecond = 6f;

    private PlayerMovement movement;
    private PlayerCombat combat;
    private Health health;
    private Sprite originalSprite;
    private VisualState currentState;
    private VisualState actionState;
    private float actionStartTime;
    private float actionEndTime;
    private float hurtStartTime;
    private float hurtEndTime;
    private float stunStartTime;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<Health>();
        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
            originalSprite = playerRenderer.sprite;
    }

    private void OnEnable()
    {
        combat.AttackPerformed += HandleAttack;
        combat.StunPerformed += HandleStunCast;
        health.OnDamaged += HandleDamage;
        currentState = VisualState.Idle;
        actionState = VisualState.Idle;
        actionEndTime = hurtEndTime = float.NegativeInfinity;
        ApplyFrame();
    }

    private void OnDisable()
    {
        combat.AttackPerformed -= HandleAttack;
        combat.StunPerformed -= HandleStunCast;
        health.OnDamaged -= HandleDamage;
        actionEndTime = hurtEndTime = float.NegativeInfinity;
    }

    private void HandleAttack()
    {
        StartAction(VisualState.Attack, attackFrames, attackFramesPerSecond);
    }

    private void HandleStunCast()
    {
        StartAction(VisualState.CastStun, stunFrames, stunFramesPerSecond);
    }

    private void StartAction(VisualState state, Sprite[] frames, float framesPerSecond)
    {
        // Visuals observe accepted gameplay actions; they never delay or block them.
        if (health.CurrentHealth <= 0f || movement.IsStunned || Time.time < hurtEndTime)
            return;

        actionEndTime = float.NegativeInfinity;
        if (frames == null || frames.Length == 0)
        {
            currentState = VisualState.Idle;
            ApplyFrame();
            return;
        }

        actionState = currentState = state;
        actionStartTime = Time.time;
        actionEndTime = Time.time + frames.Length / Mathf.Max(1f, framesPerSecond);
        UpdateFacing();
        ApplyFrame();
    }

    private void HandleDamage()
    {
        actionEndTime = float.NegativeInfinity;
        if (health.CurrentHealth <= 0f || hurtFrames == null || hurtFrames.Length == 0)
            return;

        currentState = VisualState.Hurt;
        hurtStartTime = Time.time;
        hurtEndTime = Time.time + hurtFrames.Length / Mathf.Max(1f, hurtFramesPerSecond);
        ApplyFrame();
    }

    private void LateUpdate()
    {
        // PlayerDeath owns death/scene flow; do not replace the last sprite after death.
        if (playerRenderer == null || health.CurrentHealth <= 0f || PauseManager.IsPaused)
            return;

        if (movement.IsStunned)
            actionEndTime = float.NegativeInfinity;

        VisualState nextState = SelectState();
        if (nextState == VisualState.Stunned && currentState != VisualState.Stunned)
            stunStartTime = Time.time;
        currentState = nextState;

        // Action facing stays locked visually; movement and attack direction remain untouched.
        if (currentState == VisualState.Idle)
            UpdateFacing();
        ApplyFrame();
    }

    private VisualState SelectState()
    {
        if (Time.time < hurtEndTime)
            return VisualState.Hurt;
        if (movement.IsStunned)
            return VisualState.Stunned;
        if (Time.time < actionEndTime)
            return actionState;
        return VisualState.Idle;
    }

    private void UpdateFacing()
    {
        if (playerRenderer != null && Mathf.Abs(movement.FacingDirection.x) > 0.01f)
            playerRenderer.flipX = artworkFacesRight
                ? movement.FacingDirection.x < 0f
                : movement.FacingDirection.x > 0f;
    }

    private void ApplyFrame()
    {
        if (playerRenderer == null)
            return;

        Sprite[] frames = null;
        float startTime = actionStartTime;
        float framesPerSecond = attackFramesPerSecond;
        if (currentState == VisualState.Attack)
            frames = attackFrames;
        else if (currentState == VisualState.Hurt)
        {
            frames = hurtFrames;
            startTime = hurtStartTime;
            framesPerSecond = hurtFramesPerSecond;
        }
        else if (currentState == VisualState.CastStun || currentState == VisualState.Stunned)
        {
            frames = stunFrames;
            startTime = currentState == VisualState.Stunned ? stunStartTime : actionStartTime;
            framesPerSecond = stunFramesPerSecond;
        }

        Sprite frame = idleSprite != null ? idleSprite : originalSprite;
        if (frames != null && frames.Length > 0)
        {
            int index = Mathf.FloorToInt(Mathf.Max(0f, Time.time - startTime) * Mathf.Max(1f, framesPerSecond));
            index = currentState == VisualState.Stunned
                ? index % frames.Length
                : Mathf.Min(index, frames.Length - 1);
            if (frames[index] != null)
                frame = frames[index];
        }

        // Health exclusively owns tint/flashes. Never scale the root or alter a collider.
        if (frame != null)
            playerRenderer.sprite = frame;
    }
}