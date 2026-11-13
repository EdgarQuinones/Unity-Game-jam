using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    [Header("Fill Images")]
    [SerializeField] private Image attackFill;
    [SerializeField] private Image stunFill;
    [SerializeField] private Image rollFill;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image karmaFill;

    [Header("Gameplay References (optional)")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Health playerHealth;
    [SerializeField] private LuckyReel luckyReel;
    [SerializeField] private Karma karma;

    private void Start()
    {
        if (playerCombat == null)
            playerCombat = FindFirstObjectByType<PlayerCombat>();
        if (playerHealth == null && playerCombat != null)
            playerHealth = playerCombat.GetComponent<Health>();
        if (luckyReel == null)
            luckyReel = FindFirstObjectByType<LuckyReel>();
        if (karma == null)
            karma = FindFirstObjectByType<Karma>();

        if (attackFill == null || stunFill == null || rollFill == null || healthFill == null || karmaFill == null)
            Debug.LogWarning("AbilityBar: assign all five fill images in the Inspector.", this);
        if (playerCombat == null || playerHealth == null || luckyReel == null || karma == null)
            Debug.LogWarning("AbilityBar: gameplay references are missing; check this bar is in the gameplay scene.", this);
    }

    private void Update()
    {
        if (attackFill != null && playerCombat != null)
            attackFill.fillAmount = playerCombat.AttackCooldownProgress;
        if (stunFill != null && playerCombat != null)
            stunFill.fillAmount = playerCombat.StunCooldownProgress;
        if (rollFill != null && luckyReel != null)
            rollFill.fillAmount = luckyReel.ManualSpinCooldownProgress;
        if (healthFill != null && playerHealth != null)
            healthFill.fillAmount = playerHealth.MaxHealth > 0f
                ? Mathf.Clamp01(playerHealth.CurrentHealth / playerHealth.MaxHealth) : 0f;
        if (karmaFill != null && karma != null)
            karmaFill.fillAmount = Mathf.Clamp01(karma.CurrentKarma / 100f);
    }
}