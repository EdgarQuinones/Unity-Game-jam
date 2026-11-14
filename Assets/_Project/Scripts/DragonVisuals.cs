using UnityEngine;

[RequireComponent(typeof(EnemyAI), typeof(SpriteRenderer))]
public class DragonVisuals : MonoBehaviour
{
    private enum VisualState { Idle, Charge, FireBreath, Stunned, Hurt }

    [Header("Renderer")]
    [SerializeField] private SpriteRenderer dragonRenderer;
    [Tooltip("The supplied side-view dragon frames face right before flipping.")]
    [SerializeField] private bool artworkFacesRight = true;

    [Header("Frames (in numbered order)")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] chargeFrames;
    [SerializeField] private Sprite[] fireBreathFrames;
    [SerializeField] private Sprite[] stunnedFrames;
    [SerializeField] private Sprite[] hurtFrames;
    [SerializeField, Min(0f)] private float framesPerSecond = 6f;
    [SerializeField, Min(1f)] private float hurtFramesPerSecond = 10f;

    private EnemyAI ai;
    private DragonCharge charge;
    private DragonFireBreath breath;
    private Health health;
    private Sprite originalSprite;
    private VisualState currentState;
    private float animationTime;
    private float hurtEndTime;

    private void Awake()
    {
        ai = GetComponent<EnemyAI>();
        charge = GetComponent<DragonCharge>();
        breath = GetComponent<DragonFireBreath>();
        health = GetComponent<Health>();
        if (dragonRenderer == null)
            dragonRenderer = GetComponent<SpriteRenderer>();
        if (dragonRenderer != null)
            originalSprite = dragonRenderer.sprite;
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDamaged += HandleDamage;
        hurtEndTime = 0f;
        currentState = VisualState.Idle;
        animationTime = 0f;
        ApplyFrame();
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDamaged -= HandleDamage;
        hurtEndTime = 0f;
    }

    private void HandleDamage()
    {
        if (health == null || health.CurrentHealth <= 0f ||
            ai == null || ai.CurrentState == EnemyAI.EnemyState.Stunned ||
            hurtFrames == null || hurtFrames.Length == 0)
            return;

        hurtEndTime = Time.time + hurtFrames.Length / Mathf.Max(1f, hurtFramesPerSecond);
        currentState = VisualState.Hurt;
        animationTime = 0f;
        ApplyFrame();
    }

    private void LateUpdate()
    {
        if (dragonRenderer == null || ai == null)
            return;

        VisualState nextState = SelectState();
        if (nextState != currentState)
        {
            currentState = nextState;
            animationTime = 0f;
        }
        else
        {
            animationTime += Time.deltaTime;
        }

        // Freeze facing while stunned, and keep attacks facing their locked direction.
        if (currentState != VisualState.Stunned)
        {
            Vector2 direction = Vector2.zero;
            if (charge != null && charge.IsPreparingOrCharging)
                direction = charge.AttackDirection;
            else if (breath != null && breath.IsPreparingOrBreathing)
                direction = breath.AttackDirection;
            else if (ai.Target != null && ai.Target.gameObject.activeInHierarchy)
                direction = (Vector2)(ai.Target.position - transform.position);

            if (Mathf.Abs(direction.x) > 0.01f)
                dragonRenderer.flipX = artworkFacesRight ? direction.x < 0f : direction.x > 0f;
        }

        ApplyFrame();
    }

    private VisualState SelectState()
    {
        if (ai.CurrentState == EnemyAI.EnemyState.Stunned)
            return VisualState.Stunned;
        if (Time.time < hurtEndTime)
            return VisualState.Hurt;
        if (charge != null && charge.IsPreparingOrCharging)
            return VisualState.Charge;
        if (breath != null && breath.IsPreparingOrBreathing)
            return VisualState.FireBreath;
        return VisualState.Idle;
    }

    private void ApplyFrame()
    {
        if (dragonRenderer == null)
            return;

        Sprite[] frames = idleFrames;
        if (currentState == VisualState.Charge)
            frames = chargeFrames;
        else if (currentState == VisualState.FireBreath)
            frames = fireBreathFrames;
        else if (currentState == VisualState.Stunned)
            frames = stunnedFrames;
        else if (currentState == VisualState.Hurt)
            frames = hurtFrames;

        if (frames == null || frames.Length == 0)
            frames = idleFrames;

        Sprite frame = originalSprite;
        if (frames != null && frames.Length > 0)
        {
            int index = currentState == VisualState.Hurt
                ? Mathf.Min(frames.Length - 1, Mathf.FloorToInt(animationTime * Mathf.Max(1f, hurtFramesPerSecond)))
                : Mathf.FloorToInt(animationTime * Mathf.Max(0f, framesPerSecond)) % frames.Length;
            if (frames[index] != null)
                frame = frames[index];
            else if (frames[0] != null)
                frame = frames[0];
        }

        // Do not overwrite Health's damage flash or change the root/collider transform.
        if (frame != null)
            dragonRenderer.sprite = frame;
    }
}