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
    private int resolvedEnemies = 0;
    private HashSet<EnemyAI> resolvedEnemySet = new HashSet<EnemyAI>();

    [Header("Boss Phase")]
    [SerializeField] private EnemyAI boss;

    [SerializeField] private int totalPuzzles;
    private int completedPuzzles = 0;
    private readonly List<CleaningPuzzle> cleaningPuzzles = new List<CleaningPuzzle>();
    private readonly HashSet<CleaningPuzzle> completedPuzzleSet = new HashSet<CleaningPuzzle>();

    private float timeRemaining;
    private GamePhase currentPhase;
    public GamePhase CurrentPhase => currentPhase;

    private RunManager runManager;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();

        totalPuzzles = runManager.PuzzleCount;

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

        currentPhase = GamePhase.Encounter;

        foreach (CleaningPuzzle puzzle in cleaningPuzzles)
        {
            if (puzzle != null)
                puzzle.gameObject.SetActive(false);
        }

        Debug.Log("Cleaning Phase Ended!");

        resolvedEnemies = 0;
        resolvedEnemySet.Clear();

        foreach (EnemyAI enemy in encounterEnemies)
        {
            if (enemy != null)
                enemy.gameObject.SetActive(true);
        }
    }

    public void EnemyResolved(EnemyAI enemy)
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        if (enemy == null || !resolvedEnemySet.Add(enemy))
            return;

        resolvedEnemies++;

        Debug.Log($"Enemies resolved: {resolvedEnemies}/{encounterEnemies.Length}");

        if (resolvedEnemies >= encounterEnemies.Length)
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