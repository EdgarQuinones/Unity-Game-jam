using UnityEngine;

public class PressurePlatePuzzle : MonoBehaviour
{
    [SerializeField] private int[] correctSequence = { 2, 1, 3 };

    private int currentStep = 0;
    private bool isCompleted = false;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void ActivatePlate(int plateID)
    {
        if (gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return;

        if (isCompleted) return;

        if (plateID == correctSequence[currentStep])
        {
            currentStep++;

            Debug.Log("Correct plate!");

            if (currentStep >= correctSequence.Length)
            {
                isCompleted = true;
                Debug.Log("Puzzle completed!");

                gameManager.PuzzleCompleted();
            }
        }
        else
        {
            currentStep = 0;
            Debug.Log("Wrong plate! Sequence reset.");
        }
    }
}