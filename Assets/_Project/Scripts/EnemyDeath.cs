using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private int karmaOnKill = 10;

    private Health health;
    private Karma karma;

    private GameManager gameManager;
    private EnemyAI enemyAI;

    private void Awake()
    {
        health = GetComponent<Health>();
        karma = FindFirstObjectByType<Karma>();
        gameManager = FindFirstObjectByType<GameManager>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        karma.ModifyKarma(karmaOnKill);
        if (gameManager != null)
            gameManager.EnemyResolved(enemyAI);

        if (karma != null)
        {
            karma.ModifyKarma(karmaOnKill);
        }
    }
}