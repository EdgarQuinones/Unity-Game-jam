using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GamePhase
    {
        Cleaning,
        Encounter,
        Boss,
        Complete,
        GameOver
    }

    [Header("Cleaning Phase")]
    [SerializeField] private CleaningPuzzle[] puzzleGroups;

    [Header("Encounter Phase")]
    [SerializeField] private EnemyAI[] encounterEnemies;
    [SerializeField] private Health playerHealth;
    private int resolvedEnemies = 0;
    private HashSet<EnemyAI> resolvedEnemySet = new HashSet<EnemyAI>();
    private readonly HashSet<EnemyAI> selectedEncounterEnemies = new HashSet<EnemyAI>();

    [Header("Boss Phase")]
    [SerializeField] private EnemyAI boss;

    [SerializeField] private int totalPuzzles;
    private int completedPuzzles = 0;
    private readonly List<CleaningPuzzle> cleaningPuzzles = new List<CleaningPuzzle>();
    private readonly HashSet<CleaningPuzzle> completedPuzzleSet = new HashSet<CleaningPuzzle>();

    [Header("Misc")]
    [SerializeField] private float timeRemaining;
    private GamePhase currentPhase;
    public GamePhase CurrentPhase => currentPhase;

    private RunManager runManager;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();

        totalPuzzles = runManager.PuzzleCount;

        foreach (EnemyAI enemy in encounterEnemies)
        {
            if (enemy != null && enemy != boss)
                enemy.gameObject.SetActive(false);
        }

        StartCleaningPhase();
    }

    private void Update()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndCleaningPhase();
        }
    }

    private void StartCleaningPhase()
    {
        currentPhase = GamePhase.Cleaning;
        timeRemaining = 10f + totalPuzzles * 20f;

        if (puzzleGroups == null || puzzleGroups.Length == 0)
            puzzleGroups = GetComponentsInChildren<CleaningPuzzle>(true);

        List<CleaningPuzzle> availablePuzzles = new List<CleaningPuzzle>();
        foreach (CleaningPuzzle puzzle in puzzleGroups)
        {
            if (puzzle == null)
            {
                Debug.LogError("Cleaning puzzle list contains a missing reference.", this);
                continue;
            }

            if (availablePuzzles.Contains(puzzle))
            {
                Debug.LogWarning($"Duplicate cleaning puzzle ignored: {puzzle.name}.", puzzle);
                continue;
            }

            availablePuzzles.Add(puzzle);
            puzzle.gameObject.SetActive(false);
        }

        for (int i = availablePuzzles.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CleaningPuzzle temporary = availablePuzzles[i];
            availablePuzzles[i] = availablePuzzles[randomIndex];
            availablePuzzles[randomIndex] = temporary;
        }

        int selectedCount = Mathf.Min(totalPuzzles, availablePuzzles.Count);
        for (int i = 0; i < selectedCount; i++)
        {
            CleaningPuzzle puzzle = availablePuzzles[i];
            cleaningPuzzles.Add(puzzle);
            puzzle.gameObject.SetActive(true);
            //Debug.Log($"Randomly selected cleaning puzzle: {puzzle.name}.", puzzle);
        }

        if (availablePuzzles.Count < totalPuzzles)
            Debug.LogError($"Cleaning requires {totalPuzzles} puzzles but only {availablePuzzles.Count} unique puzzles are assigned; completion is not possible before timeout.", this);

        Debug.Log($"Cleaning Phase Started! Required puzzles: {totalPuzzles}; time: {timeRemaining} seconds.");
    }

    public void EndCleaningPhase()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        if (timeRemaining <= 0f)
        {
            int unfinishedPuzzles = Mathf.Max(0, totalPuzzles - completedPuzzles);
            if (unfinishedPuzzles > 0)
            {
                CatHappiness catHappiness = FindFirstObjectByType<CatHappiness>();
                if (catHappiness != null)
                {
                    catHappiness.ModifyHappiness(-10 * unfinishedPuzzles);
                    Debug.Log($"Cleaning timed out with {unfinishedPuzzles} unfinished puzzles; Happiness penalty: {-10 * unfinishedPuzzles}.");
                }
                else
                {
                    Debug.LogError("Cannot apply Cleaning timeout penalty: CatHappiness not found.", this);
                }
            }
        }

        currentPhase = GamePhase.Encounter;

        foreach (CleaningPuzzle puzzle in cleaningPuzzles)
        {
            if (puzzle != null)
                puzzle.gameObject.SetActive(false);
        }

        Debug.Log("Cleaning Phase Ended!");

        resolvedEnemies = 0;
        resolvedEnemySet.Clear();
        selectedEncounterEnemies.Clear();

        List<EnemyAI> availableEnemies = new List<EnemyAI>();
        foreach (EnemyAI enemy in encounterEnemies)
        {
            if (enemy == null || enemy == boss || enemy.IsBoss)
            {
                Debug.LogError("Encounter enemy list contains a missing reference or a boss.", this);
                continue;
            }

            if (availableEnemies.Contains(enemy))
            {
                Debug.LogWarning($"Duplicate encounter enemy ignored: {enemy.name}.", enemy);
                continue;
            }

            availableEnemies.Add(enemy);
        }

        for (int i = availableEnemies.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            EnemyAI temporary = availableEnemies[i];
            availableEnemies[i] = availableEnemies[randomIndex];
            availableEnemies[randomIndex] = temporary;
        }

        int selectedCount = Mathf.Min(runManager.EnemyCount, availableEnemies.Count);
        if (selectedCount < runManager.EnemyCount)
            Debug.LogError($"Encounter requires {runManager.EnemyCount} enemies but only {availableEnemies.Count} unique valid enemies are assigned.", this);

        for (int i = 0; i < selectedCount; i++)
            selectedEncounterEnemies.Add(availableEnemies[i]);

        //Debug.Log($"Encounter roll: {runManager.EncounterRoll}; selected {selectedCount} enemies.");

        if (selectedCount == 0)
        {
            Debug.LogError("No valid encounter enemies are assigned; cannot start the encounter.", this);
            return;
        }

        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            Debug.Log($"Encounter enemy activated: {enemy.name}.", enemy);
            enemy.gameObject.SetActive(true);
        }
    }

    public void HealPlayerFromEncounter()
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        if (playerHealth == null)
        {
            Debug.LogWarning("Cannot heal player: Player Health is not assigned.", this);
            return;
        }

        float healthBefore = playerHealth.CurrentHealth;
        playerHealth.Heal(20f);
        Debug.Log($"Encounter reel healed player by {playerHealth.CurrentHealth - healthBefore:0.##}; health: {playerHealth.CurrentHealth:0.##}/{playerHealth.MaxHealth:0.##}.", playerHealth);
    }

    public void BoostEncounterEnemySpeed()
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                continue;

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
                movement.ApplySpeedBoost(1.5f, 8f);
            else
                Debug.LogWarning($"{enemy.name} cannot receive the speed boost: EnemyMovement is missing.", enemy);
        }
    }

    public void ApplyGoodEncounterFortune(int karmaDebt)
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        if (karmaDebt < 45)
        {
            List<EnemyAI> eligibleEnemies = new List<EnemyAI>();
            List<EnemyAI> alreadyStunnedEnemies = new List<EnemyAI>();
            foreach (EnemyAI enemy in selectedEncounterEnemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                    continue;

                if (enemy.CanBeSealed)
                    alreadyStunnedEnemies.Add(enemy);
                else
                    eligibleEnemies.Add(enemy);
            }

            if (eligibleEnemies.Count == 0)
                eligibleEnemies = alreadyStunnedEnemies;

            if (eligibleEnemies.Count == 0)
            {
                Debug.LogWarning("Good Encounter fortune found no active enemies to stun.", this);
                return;
            }

            EnemyAI target = eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];
            target.Stun(8f, EnemyAI.StunType.Environmental);
            Debug.Log($"Good Encounter fortune environmentally stuns {target.name} for at least 8 seconds.", target);
            return;
        }

        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                continue;

            if (karmaDebt > 55)
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth == null)
                {
                    Debug.LogWarning($"{enemy.name} cannot receive reel damage: Health is missing.", enemy);
                    continue;
                }

                Debug.Log($"Good Encounter fortune deals 20 damage to {enemy.name}.", enemy);
                enemyHealth.TakeDamage(20f);
            }
            else
            {
                enemy.Stun(3f, EnemyAI.StunType.Player);
                Debug.Log($"Good Encounter fortune regularly stuns {enemy.name} for at least 3 seconds.", enemy);
            }
        }
    }

    public void EnemyResolved(EnemyAI enemy)
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        if (enemy == null || !selectedEncounterEnemies.Contains(enemy) || !resolvedEnemySet.Add(enemy))
            return;

        resolvedEnemies++;

        Debug.Log($"Enemies resolved: {resolvedEnemies}/{selectedEncounterEnemies.Count}");

        if (resolvedEnemies >= selectedEncounterEnemies.Count)
        {
            EndEncounterPhase();
        }
    }

    private void EndEncounterPhase()
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        currentPhase = GamePhase.Boss;

        Debug.Log("Encounter Complete!");
        Debug.Log("Boss Phase Started!");

        if (boss != null)
            boss.gameObject.SetActive(true);
    }

    public void AdjustCleaningTime(float seconds)
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        timeRemaining = Mathf.Max(0f, timeRemaining + seconds);
        Debug.Log($"Cleaning time adjusted by {seconds:+0.##;-0.##;0} seconds; remaining: {timeRemaining:F1} seconds.");

        if (timeRemaining <= 0f)
        {
            Debug.Log("Cleaning time ran out due to a Lucky Reel outcome.");
            EndCleaningPhase();
        }
    }

    public void CompleteRandomCleaningPuzzle()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        List<CleaningPuzzle> unfinished = new List<CleaningPuzzle>();
        foreach (CleaningPuzzle puzzle in cleaningPuzzles)
        {
            if (puzzle != null && !completedPuzzleSet.Contains(puzzle))
                unfinished.Add(puzzle);
        }

        if (unfinished.Count == 0)
        {
            Debug.LogWarning("Lucky Reel could not complete a puzzle: no unfinished selected puzzles remain.", this);
            return;
        }

        CleaningPuzzle selected = unfinished[Random.Range(0, unfinished.Count)];
        Debug.Log($"Lucky Reel selected {selected.name} for automatic completion.", selected);
        selected.CompleteFromLuckyReel();
    }

    public void PuzzleCompleted(CleaningPuzzle puzzle)
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        if (puzzle == null || !cleaningPuzzles.Contains(puzzle) || !completedPuzzleSet.Add(puzzle))
        {
            Debug.LogWarning("Ignored completion from a missing, unselected, or already completed cleaning puzzle.", this);
            return;
        }

        completedPuzzles++;

        Debug.Log($"Completed: {completedPuzzles}/{totalPuzzles}");

        if (completedPuzzles >= totalPuzzles)
        {
            EndCleaningPhase();
        }
    }

    public void BossResolved()
    {
        if (currentPhase != GamePhase.Boss)
            return;

        currentPhase = GamePhase.Complete;

        Debug.Log("Boss Defeated!");
        Debug.Log("Level Complete!");

        // Later: Activate Lucky Slot Machine
    }

    public void GameOver()
    {
        if (currentPhase == GamePhase.GameOver ||
            currentPhase == GamePhase.Complete)
            return;

        currentPhase = GamePhase.GameOver;

        Debug.Log("GAME OVER!");
    }

}