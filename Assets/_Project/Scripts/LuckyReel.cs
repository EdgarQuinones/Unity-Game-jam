
using UnityEngine;
using UnityEngine.InputSystem;

public class LuckyReel : MonoBehaviour
{
    private enum LuckOutcome
    {
        Bad,
        Neutral,
        Good
    }

    [Header("Spin Timers")]
    [SerializeField] private float manualCooldown = 12f;
    [SerializeField] private float autoSpinInterval = 30f;
    [SerializeField] private float minimumSpinGap = 5f;

    [Header("Happiness Odds")]
    [SerializeField, Range(0f, 0.9f)] private float happinessBiasStrength = 0.8f;

    public int Reel1 { get; private set; }
    public int Reel2 { get; private set; }
    public int Reel3 { get; private set; }

    public int Total => Reel1 + Reel2 + Reel3;

    private float nextManualSpinTime;
    private float nextAutoSpinTime;
    private float lastSpinTime = float.NegativeInfinity;
    private GameManager gameManager;
    private CatHappiness catHappiness;
    private Karma karma;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        catHappiness = GetComponent<CatHappiness>();
        karma = GetComponent<Karma>();

        if (catHappiness == null)
            Debug.LogWarning("LuckyReel requires CatHappiness on the same GameObject; using neutral odds.", this);

        if (karma == null)
            Debug.LogWarning("LuckyReel requires Karma on the same GameObject to select good combat fortune.", this);
    }

    private void Start()
    {
        // First automatic spin occurs after 30 seconds
        nextAutoSpinTime = Time.time + autoSpinInterval;
    }

    private void Update()
    {
        // Automatic spin is due
        if (Time.time < nextAutoSpinTime)
            return;

        // Wait if another spin happened recently
        if (!CanSpin())
            return;

        Spin();

        nextAutoSpinTime = Time.time + autoSpinInterval;
    }

    // Called by Q through Unity Events
    public void OnLuckySpin(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // Manual cooldown
        if (Time.time < nextManualSpinTime)
            return;

        // Shared protection between spins
        if (!CanSpin())
            return;

        Spin();

        nextManualSpinTime = Time.time + manualCooldown;
    }

    private bool CanSpin()
    {
        if (gameManager == null)
            return false;

        GameManager.GamePhase phase = gameManager.CurrentPhase;

        if (phase != GameManager.GamePhase.Cleaning &&
            phase != GameManager.GamePhase.Encounter &&
            phase != GameManager.GamePhase.Boss)
            return false;

        return Time.time >= lastSpinTime + minimumSpinGap;
    }

    private void Spin()
    {
        int happiness = catHappiness != null ? catHappiness.CurrentHappiness : 50;
        float bias = (Mathf.Clamp(happiness, 0, 100) - 50f) / 50f;
        bias *= Mathf.Clamp(happinessBiasStrength, 0f, 0.9f);
        Reel1 = RollNumber(bias);
        Reel2 = RollNumber(bias);
        Reel3 = RollNumber(bias);

        lastSpinTime = Time.time;

        float lowChance = (3f - 2.25f * bias) / 9f;
        float highChance = (3f + 2.25f * bias) / 9f;
        Debug.Log($"Lucky odds: Happiness {happiness}, bias {bias:F2}; " +
                  $"per reel 1-3: {lowChance:P1}, 7-9: {highChance:P1}");
        Debug.Log($"Lucky Spin: {Reel1}-{Reel2}-{Reel3}");
        Debug.Log($"Total: {Total}");

        if (Reel1 == Reel2 && Reel2 == Reel3)
        {
            Debug.Log("TRIPLE MATCH!");
        }

        LuckOutcome outcome = GetOutcome();
        RouteOutcome(outcome);
    }

    private int RollNumber(float bias)
    {
        // At 50 all numbers are equally likely. Keep every weight positive so
        // any combination remains possible even at extreme happiness levels.
        float roll = Random.value * 9f;

        for (int number = 1; number <= 9; number++)
        {
            float weight = 1f + bias * (number - 5) / 4f;
            if (roll < weight)
                return number;

            roll -= weight;
        }

        // Random.value can include its upper endpoint.
        return 9;
    }

    private LuckOutcome GetOutcome()
    {
        float average = Total / 3f;

        if (average < 4f)
            return LuckOutcome.Bad;

        if (average < 7f)
            return LuckOutcome.Neutral;

        return LuckOutcome.Good;
    }

    private void RouteOutcome(LuckOutcome outcome)
    {
        switch (gameManager.CurrentPhase)
        {
            case GameManager.GamePhase.Cleaning:
                Debug.Log($"Cleaning outcome: {outcome}");
                break;

            case GameManager.GamePhase.Encounter:
                Debug.Log($"Encounter outcome: {outcome}");
                if (outcome == LuckOutcome.Good)
                    LogCombatFortune();
                break;

            case GameManager.GamePhase.Boss:
                Debug.Log($"Boss outcome: {outcome}");
                if (outcome == LuckOutcome.Good)
                    LogCombatFortune();
                break;
        }
    }

    private void LogCombatFortune()
    {
        if (karma == null)
        {
            Debug.LogWarning("Cannot select combat fortune: Karma is missing.", this);
            return;
        }

        string fortuneType = karma.CurrentKarma >= 50 ? "Offensive" : "Peaceful";
        Debug.Log($"Karma Debt: {karma.CurrentKarma} -> {fortuneType} fortune selected (effect not implemented).");
    }
}
