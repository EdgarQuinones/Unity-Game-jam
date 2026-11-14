using UnityEngine;
using UnityEngine.UI;

public class CatHappinessUI : MonoBehaviour
{
    [Header("Gameplay Reference (optional)")]
    [SerializeField] private CatHappiness happinessSource;

    [Header("Images")]
    [SerializeField] private Image happinessFill;
    [SerializeField] private Image catImage;

    [Header("Expressions")]
    [SerializeField] private Sprite lowHappinessSprite;
    [SerializeField] private Sprite neutralHappinessSprite;
    [SerializeField] private Sprite highHappinessSprite;

    [Header("Expression Thresholds")]
    [Tooltip("The first Happiness value that uses the neutral expression.")]
    [SerializeField, Range(1, 99)] private int neutralStartsAt = 34;
    [Tooltip("The first Happiness value that uses the high expression.")]
    [SerializeField, Range(2, 100)] private int highStartsAt = 67;

    private void OnValidate()
    {
        neutralStartsAt = Mathf.Clamp(neutralStartsAt, 1, 99);
        highStartsAt = Mathf.Clamp(highStartsAt, neutralStartsAt + 1, 100);
    }

    private void Start()
    {
        if (happinessSource == null)
            happinessSource = FindFirstObjectByType<CatHappiness>();

        if (happinessSource == null)
            Debug.LogWarning("CatHappinessUI: assign the existing CatHappiness component from the gameplay manager.", this);
        if (happinessFill == null || catImage == null)
            Debug.LogWarning("CatHappinessUI: assign the Happiness Fill and Cat Image.", this);
        if (lowHappinessSprite == null || neutralHappinessSprite == null || highHappinessSprite == null)
            Debug.LogWarning("CatHappinessUI: assign all three expression sprites.", this);

        if (happinessFill != null)
            happinessFill.raycastTarget = false;
        if (catImage != null)
            catImage.raycastTarget = false;

        RefreshDisplay();
    }

    private void Update()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (happinessSource == null)
            return;

        int happiness = Mathf.Clamp(happinessSource.CurrentHappiness, 0, 100);
        if (happinessFill != null)
            happinessFill.fillAmount = happiness / 100f;

        Sprite expression = happiness < neutralStartsAt
            ? lowHappinessSprite
            : happiness < highStartsAt ? neutralHappinessSprite : highHappinessSprite;

        if (catImage != null && expression != null && catImage.sprite != expression)
            catImage.sprite = expression;
    }
}