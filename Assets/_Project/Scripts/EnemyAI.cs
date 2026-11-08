using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Attack,
        Stunned
    }

    [Header("AI Settings")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField] private Transform player;

    private float stunEndTime;

    private EnemyState currentState = EnemyState.Chase;

    public EnemyState CurrentState => currentState;
    public Transform Target => player;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (Time.time < stunEndTime)
        {
            currentState = EnemyState.Stunned;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else
        {
            currentState = EnemyState.Chase;
        }
    }

    public void Stun(float duration)
    {
        stunEndTime = Mathf.Max(stunEndTime, Time.time + duration);
        currentState = EnemyState.Stunned;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (currentState == EnemyState.Stunned)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }

}