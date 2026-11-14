
using UnityEngine;

public class RunManager : MonoBehaviour
{
    private static bool hasPendingFortune;
    private static int pendingCleaningRoll;
    private static int pendingEncounterRoll;
    private static int pendingBossRoll;

    public int CleaningRoll { get; private set; }
    public int EncounterRoll { get; private set; }
    public int BossRoll { get; private set; }

    public int PuzzleCount { get; private set; }
    public int EnemyCount { get; private set; }
    public float BossSpeedMultiplier { get; private set; }

    private void Awake()
    {
        if (hasPendingFortune)
        {
            SetFortune(pendingCleaningRoll, pendingEncounterRoll, pendingBossRoll);
            hasPendingFortune = false;
        }
        else
        {
            SetFortune(Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ClearPendingOnPlay()
    {
        hasPendingFortune = false;
    }

    public static void PrepareFortune(out int cleaning, out int encounter, out int boss)
    {
        cleaning = pendingCleaningRoll = Random.Range(1, 10);
        encounter = pendingEncounterRoll = Random.Range(1, 10);
        boss = pendingBossRoll = Random.Range(1, 10);
        hasPendingFortune = true;
    }

    public static void ClearPendingFortune()
    {
        hasPendingFortune = false;
    }

    private void SetFortune(int cleaning, int encounter, int boss)
    {
        CleaningRoll = cleaning;
        EncounterRoll = encounter;
        BossRoll = boss;

        PuzzleCount = PuzzleCountForRoll(CleaningRoll);

        EnemyCount = EnemyCountForRoll(EncounterRoll);

        BossSpeedMultiplier = BossSpeedMultiplierForRoll(BossRoll);

        Debug.Log($"Cleaning: {CleaningRoll} -> {PuzzleCount} puzzles");
        Debug.Log($"Encounter: {EncounterRoll} -> {EnemyCount} enemies");
        Debug.Log($"Boss: {BossRoll} -> {BossSpeedMultiplier}x attack speed");
    }

    public static int PuzzleCountForRoll(int roll)
    {
        return roll <= 3 ? 3 : roll <= 6 ? 2 : 1;
    }

    public static int EnemyCountForRoll(int roll)
    {
        return roll <= 3 ? 6 : roll <= 6 ? 4 : 2;
    }

    public static float BossSpeedMultiplierForRoll(int roll)
    {
        return roll <= 3 ? 1.25f : roll <= 6 ? 1f : 0.75f;
    }
}
