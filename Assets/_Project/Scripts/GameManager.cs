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
    [SerializeField] private float cleaningDuration = 60f;

    [Header("Encounter Phase")]
    [SerializeField] private EnemyAI[] encounterEnemies;
    private int resolvedEnemies = 0;
    private HashSet<EnemyAI> resolvedEnemySet = new HashSet<EnemyAI>();

    [Header("Boss Phase")]
    [SerializeField] private EnemyAI boss;

    [SerializeField] private int totalPuzzles;
    private int completedPuzzles = 0;

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
        timeRemaining = cleaningDuration;

        Debug.Log("Cleaning Phase Started!");
    }

    public void EndCleaningPhase()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        currentPhase = GamePhase.Encounter;

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

    public void PuzzleCompleted()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

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