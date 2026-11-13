using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Attack,
        Stunned
    }

    public enum StunType
    {
        Player,
        Environmental
    }

    [Header("AI Settings")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField] private Transform player;

    private float stunEndTime = 0f;
    private bool isSealed = false;

    private Karma karma;

    private EnemyState currentState = EnemyState.Chase;

    public EnemyState CurrentState => currentState;
    public Transform Target => player;
    public StunType CurrentStunType { get; private set; }

    [SerializeField] private bool isBoss = false;
    [SerializeField, Range(0f, 1f)] private float bossSealHealthThreshold = 0.25f;
    private Health health;
    public bool IsBoss => isBoss;
    private bool IsEnvironmentallyStunned =>
        !isSealed &&
        currentState == EnemyState.Stunned &&
        CurrentStunType == StunType.Environmental &&
        Time.time < stunEndTime;
    public bool CanBeSealed => IsEnvironmentallyStunned &&
        (!isBoss || (health != null && health.CurrentHealth > 0f &&
                     health.CurrentHealth <= health.MaxHealth * bossSealHealthThreshold));

    GameManager gameManager;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        gameManager = FindFirstObjectByType<GameManager>();

        karma = FindFirstObjectByType<Karma>();
    }

    private void Update()
    {
        if (isSealed)
            return;

        // Stunned enemies cannot chase or attack
        if (Time.time < stunEndTime)
        {
            currentState = EnemyState.Stunned;
            return;
        }

        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackRange)
            currentState = EnemyState.Attack;
        else
            currentState = EnemyState.Chase;
    }

    public void Stun(
        float duration,
        StunType type = StunType.Player
    )
    {
        if (duration <= 0f || isSealed)
            return;

        // Player stun cannot overwrite environmental stun
        if (IsEnvironmentallyStunned && type == StunType.Player)
            return;

        stunEndTime = Mathf.Max(
            stunEndTime,
            Time.time + duration
        );

        CurrentStunType = type;
        currentState = EnemyState.Stunned;

        //Debug.Log($"{gameObject.name} stunned by {type}");
    }

    public void Seal()
    {
        if (!CanBeSealed)
            return;
    
        if (gameManager != null)
            if (isBoss)
                gameManager.BossResolved();
            else
                gameManager.EnemyResolved(this);
    
        isSealed = true;

        if (karma != null)
            karma.ModifyKarma(-10);

        //Debug.Log($"{gameObject.name} was sealed!");

        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (currentState == EnemyState.Stunned)
        {
            Gizmos.color = CanBeSealed
                ? Color.green
                : Color.cyan;

            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }

    private void OnEnable()
    {
        // Temporary testing only
        //Debug.Log($"ENEMY ENVIRONMENTAL STUN (DEV ONLY)!");
        //Stun(10f, StunType.Environmental);
    }

    public void WakeUp()
    {
        if (currentState != EnemyState.Stunned)
            return;

        // A boss's environmental stun is a damage window and cannot be ended by a hit.
        if (isBoss && IsEnvironmentallyStunned)
            return;

        stunEndTime = 0f;
        currentState = EnemyState.Chase;

        //Debug.Log($"{gameObject.name} woke up!");
    }

}