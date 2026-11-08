using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0 || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        //Debug.Log($"{gameObject.name} took {damage} damage. Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
            return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    private void Die()
    {
        gameObject.SetActive(false);

        // Later
            // Specific action for player death
            // Specific action for enemy death

        //OnDeath?.Invoke();


        // FOR ENEMY
        //Debug.Log($"{gameObject.name} died!");

        //// Play death animation
        //// Drop XP
        //// Update Karma

        //Destroy(gameObject);
    }
}