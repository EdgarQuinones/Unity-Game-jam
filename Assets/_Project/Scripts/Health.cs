using UnityEngine;
using System.Collections;
using System;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;

    private Color originalColor;
    private Color baseColor;
    private Coroutine flashCoroutine;

    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            baseColor = originalColor;
        }
    }

    public void SetSpeedBoostVisual(bool boosted)
    {
        baseColor = boosted ? Color.yellow : originalColor;
        if (spriteRenderer != null && flashCoroutine == null)
            spriteRenderer.color = baseColor;
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        baseColor = originalColor;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        //Debug.Log($"{gameObject.name} took {damage} damage! Health: {CurrentHealth}/{MaxHealth}");

        // Wake up stunned enemies when damaged
        EnemyAI enemyAI = GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.WakeUp();
        }

        // Flash red when hit
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            spriteRenderer.color = baseColor;
            flashCoroutine = StartCoroutine(FlashColor(Color.red));
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
            return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

        if (CurrentHealth > previousHealth && spriteRenderer != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = StartCoroutine(FlashColor(Color.green));
        }
    }

    private IEnumerator FlashColor(Color color)
    {
        spriteRenderer.color = color;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = baseColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        gameObject.SetActive(false);
    }
}