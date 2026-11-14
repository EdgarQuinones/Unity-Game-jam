using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    [Header("Fill Images")]
    [SerializeField] private Image attackFill;
    [SerializeField] private Image stunFill;
    [SerializeField] private Image rollFill;
    [SerializeField] private Image healthFill;
    [Tooltip("The fixed Karma scale artwork; kept fully visible instead of filling.")]
    [SerializeField] private Image karmaFill;

    [Header("Karma Selector")]
    [SerializeField] private RectTransform karmaSelector;
    [Tooltip("Seven positions, ordered from lowest Karma on the left to highest on the right.")]
    [SerializeField] private RectTransform[] karmaSlots = new RectTransform[7];

    [Header("Gameplay References (optional)")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Health playerHealth;
    [SerializeField] private LuckyReel luckyReel;
    [SerializeField] private Karma karma;

    private bool karmaSelectorReady;

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
            Debug.LogWarning("AbilityBar: assign the four fill images and the fixed Karma scale image in the Inspector.", this);
        if (playerCombat == null || playerHealth == null || luckyReel == null || karma == null)
            Debug.LogWarning("AbilityBar: gameplay references are missing; check this bar is in the gameplay scene.", this);

        if (karmaFill != null)
        {
            karmaFill.fillAmount = 1f;
            karmaFill.raycastTarget = false;
        }

        karmaSelectorReady = karmaSelector != null && karmaSlots != null && karmaSlots.Length == 7;
        if (karmaSelectorReady)
        {
            foreach (RectTransform slot in karmaSlots)
            {
                if (slot == null)
                {
                    karmaSelectorReady = false;
                    break;
                }
            }
        }

        if (!karmaSelectorReady)
            Debug.LogWarning("AbilityBar: assign the Karma Selector and all seven Karma Slots in left-to-right order.", this);

        if (karmaSelector != null)
        {
            Image selectorImage = karmaSelector.GetComponent<Image>();
            if (selectorImage != null)
                selectorImage.raycastTarget = false;
        }
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
    }

    private void LateUpdate()
    {
        if (karmaSelectorReady && karma != null)
            karmaSelector.position = karmaSlots[KarmaSlotIndex(karma.CurrentKarma)].position;
    }

    private static int KarmaSlotIndex(int value)
    {
        if (value <= 14) return 0;
        if (value <= 29) return 1;
        if (value <= 44) return 2;
        if (value <= 55) return 3;
        if (value <= 70) return 4;
        if (value <= 85) return 5;
        return 6;
    }
}