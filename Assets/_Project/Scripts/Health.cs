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

    [Header("Regular Enemy Stun Feedback")]
    [SerializeField] private Color stunnedColor = new Color(0.4f, 0.85f, 1f);
    [SerializeField] private Color sealableColor = new Color(0.45f, 1f, 0.5f);

    private Color originalColor;
    private Color baseColor;
    private Coroutine flashCoroutine;
    private EnemyAI enemyAI;
    private bool speedBoosted;

    public event Action OnDeath;
    public event Action OnDamaged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();

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
        speedBoosted = boosted;
        RefreshBaseColor();
    }

    private void LateUpdate()
    {
        // Health owns sprite colors so stuns never compete with damage/healing flashes.
        // The dragon keeps its dedicated animation and existing color behavior.
        if (enemyAI != null && !enemyAI.IsBoss)
            RefreshBaseColor();
    }

    private void RefreshBaseColor()
    {
        baseColor = speedBoosted ? Color.yellow : originalColor;
        if (isActiveAndEnabled && enemyAI != null && !enemyAI.IsBoss && enemyAI.HasActiveStun)
        {
            baseColor = enemyAI.CanBeSealed ? sealableColor : stunnedColor;
            baseColor.a = originalColor.a;
        }

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

        speedBoosted = false;
        baseColor = originalColor;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
            return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        Debug.Log($"{gameObject.name} took {damage} damage! Health: {CurrentHealth}/{MaxHealth}");

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

        if (CurrentHealth < previousHealth)
            OnDamaged?.Invoke();

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