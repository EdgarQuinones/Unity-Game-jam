using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunStatusUI : MonoBehaviour
{
    [Header("Source (found automatically if empty)")]
    [SerializeField] private GameManager gameManager;

    [Header("Cleaning")]
    [SerializeField] private GameObject cleaningPanel;
    [SerializeField] private TMP_Text cleaningText;
    [SerializeField, Min(0f)] private float urgentSeconds = 10f;
    [SerializeField] private Color normalColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private Color urgentColor = new Color(1f, 0.35f, 0.25f);

    [Header("Boss")]
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private Image bossHealthFill;

    private Health bossHealth;
    private int lastSeconds = -1;
    private int lastCompleted = -1;
    private int lastTotal = -1;
    private int lastHealth = -1;
    private int lastMaxHealth = -1;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            Debug.LogWarning("RunStatusUI: gameplay GameManager is missing; status panels will stay hidden.", this);
        if (cleaningPanel == null || cleaningText == null || bossPanel == null || bossHealthFill == null)
            Debug.LogWarning("RunStatusUI: check Cleaning panel/text and Boss panel/fill references.", this);

        if (cleaningText != null) cleaningText.raycastTarget = false;
        if (bossHealthFill != null) bossHealthFill.raycastTarget = false;
    }

    private void LateUpdate()
    {
        bool cleaning = gameManager != null && gameManager.CurrentPhase == GameManager.GamePhase.Cleaning;
        bool bossFight = gameManager != null && gameManager.CurrentPhase == GameManager.GamePhase.Boss;

        SetVisible(cleaningPanel, cleaning);
        if (cleaning && cleaningText != null)
        {
            int seconds = Mathf.CeilToInt(gameManager.CleaningTimeRemaining);
            int completed = gameManager.CompletedCleaningPuzzles;
            int total = gameManager.TotalCleaningPuzzles;
            if (seconds != lastSeconds || completed != lastCompleted || total != lastTotal)
            {
                cleaningText.text = $"CLEANING  {seconds / 60:00}:{seconds % 60:00}\nPuzzles: {completed} / {total}";
                lastSeconds = seconds;
                lastCompleted = completed;
                lastTotal = total;
            }
            cleaningText.color = gameManager.CleaningTimeRemaining <= urgentSeconds ? urgentColor : normalColor;
        }

        // The boss starts inactive. Resolve its assigned Health without relying on active-object searches.
        if (bossFight && bossHealth == null)
            bossHealth = gameManager.BossHealth;

        bool showBoss = bossFight && bossHealth != null && bossHealth.gameObject.activeInHierarchy;
        SetVisible(bossPanel, showBoss);
        if (!showBoss)
            return;

        if (bossHealthFill != null)
            bossHealthFill.fillAmount = bossHealth.MaxHealth > 0f
                ? Mathf.Clamp01(bossHealth.CurrentHealth / bossHealth.MaxHealth) : 0f;

        int health = Mathf.CeilToInt(Mathf.Max(0f, bossHealth.CurrentHealth));
        int maximum = Mathf.CeilToInt(Mathf.Max(0f, bossHealth.MaxHealth));
        if ((health != lastHealth || maximum != lastMaxHealth))
        {
            lastHealth = health;
            lastMaxHealth = maximum;
        }
    }

    private void SetVisible(GameObject panel, bool visible)
    {
        // Never disable the controller itself: it must keep checking phase changes.
        if (panel != null && panel != gameObject && panel.activeSelf != visible)
            panel.SetActive(visible);
    }
}