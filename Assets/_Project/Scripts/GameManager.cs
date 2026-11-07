using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GamePhase
    {
        Cleaning,
        Encounter,
        Boss,
        Complete
    }

    [Header("Cleaning Phase")]
    [SerializeField] private float cleaningDuration = 60f;

    [SerializeField] private int totalPuzzles = 1;
    private int completedPuzzles = 0;

    private float timeRemaining;
    private GamePhase currentPhase;
    public GamePhase CurrentPhase => currentPhase;

    private void Start()
    {
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
        Debug.Log("Encounter Phase Started!");
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

}