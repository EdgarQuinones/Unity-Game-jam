
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemyAI))]
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
        enemyAI = GetComponent<EnemyAI>();

        karma = FindFirstObjectByType<Karma>();
        gameManager = FindFirstObjectByType<GameManager>();
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
        if (karma != null)
            karma.ModifyKarma(karmaOnKill);

        if (gameManager == null)
            return;

        if (enemyAI.IsBoss)
            gameManager.BossResolved();
        else
            gameManager.EnemyResolved(enemyAI);
    }
}
