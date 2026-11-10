
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public int CleaningRoll { get; private set; }
    public int EncounterRoll { get; private set; }
    public int BossRoll { get; private set; }

    public int PuzzleCount { get; private set; }
    public int EnemyCount { get; private set; }
    public float BossSpeedMultiplier { get; private set; }

    private void Awake()
    {
        GenerateFortune();
    }

    private void GenerateFortune()
    {
        CleaningRoll = Random.Range(1, 10);
        EncounterRoll = Random.Range(1, 10);
        BossRoll = Random.Range(1, 10);

        PuzzleCount = CleaningRoll <= 3 ? 3 :
                      CleaningRoll <= 6 ? 2 : 1;

        EnemyCount = EncounterRoll <= 3 ? 4 :
                     EncounterRoll <= 6 ? 3 : 2;

        BossSpeedMultiplier = BossRoll <= 3 ? 1.25f :
                              BossRoll <= 6 ? 1f : 0.75f;

        //Debug.Log($"Cleaning: {CleaningRoll} -> {PuzzleCount} puzzles");
        //Debug.Log($"Encounter: {EncounterRoll} -> {EnemyCount} enemies");
        //Debug.Log($"Boss: {BossRoll} -> {BossSpeedMultiplier}x attack speed");
    }
}
