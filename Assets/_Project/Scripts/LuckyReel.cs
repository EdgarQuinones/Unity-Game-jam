
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

    public int Reel1 { get; private set; }
    public int Reel2 { get; private set; }
    public int Reel3 { get; private set; }

    public int Total => Reel1 + Reel2 + Reel3;

    private float nextManualSpinTime;
    private float nextAutoSpinTime;
    private float lastSpinTime = float.NegativeInfinity;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
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
        Reel1 = Random.Range(1, 10);
        Reel2 = Random.Range(1, 10);
        Reel3 = Random.Range(1, 10);

        lastSpinTime = Time.time;

        Debug.Log($"Lucky Spin: {Reel1}-{Reel2}-{Reel3}");
        Debug.Log($"Total: {Total}");

        if (Reel1 == Reel2 && Reel2 == Reel3)
        {
            Debug.Log("TRIPLE MATCH!");
        }

        LuckOutcome outcome = GetOutcome();
        RouteOutcome(outcome);
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
                break;

            case GameManager.GamePhase.Boss:
                Debug.Log($"Boss outcome: {outcome}");
                break;
        }
    }
}
