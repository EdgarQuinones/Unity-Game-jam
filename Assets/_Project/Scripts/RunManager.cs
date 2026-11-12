
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

        PuzzleCount = CleaningRoll <= 3 ? 3 :
                      CleaningRoll <= 6 ? 2 : 1;

        EnemyCount = EncounterRoll <= 3 ? 4 :
                     EncounterRoll <= 6 ? 3 : 2;

        BossSpeedMultiplier = BossRoll <= 3 ? 1.25f :
                              BossRoll <= 6 ? 1f : 0.75f;

        Debug.Log($"Cleaning: {CleaningRoll} -> {PuzzleCount} puzzles");
        Debug.Log($"Encounter: {EncounterRoll} -> {EnemyCount} enemies");
        Debug.Log($"Boss: {BossRoll} -> {BossSpeedMultiplier}x attack speed");
    }
}
