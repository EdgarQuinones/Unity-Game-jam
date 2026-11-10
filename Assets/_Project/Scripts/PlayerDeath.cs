
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    private Health health;
    private GameManager gameManager;

    private void Awake()
    {
        health = GetComponent<Health>();
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
        Debug.Log("Player Died!");

        if (gameManager != null)
            gameManager.GameOver();
    }
}
