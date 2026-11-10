
using UnityEngine;
using UnityEngine.InputSystem;

public class LuckyReel : MonoBehaviour
{
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
    }
}
