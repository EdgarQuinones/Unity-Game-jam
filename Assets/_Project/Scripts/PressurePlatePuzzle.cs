using UnityEngine;

public class PressurePlatePuzzle : MonoBehaviour
{
    [SerializeField] private int[] correctSequence = { 2, 1, 3 };

    [SerializeField] private PressurePlate[] plates;

    private CatHappiness catHappiness;

    private int currentStep = 0;

    private bool isCompleted = false;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        catHappiness = FindFirstObjectByType<CatHappiness>();
    }

    public bool ActivatePlate(int plateID)
    {
        if (isCompleted ||
            gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return false;

        if (plateID == correctSequence[currentStep])
        {
            currentStep++;


            if (currentStep >= correctSequence.Length)
            {
                isCompleted = true;
                Debug.Log("Puzzle completed!");

                catHappiness.ModifyHappiness(10);

                gameManager.PuzzleCompleted();
            }

            return true;
        }
        else
        {
            currentStep = 0;

            catHappiness.ModifyHappiness(-5);

            ResetAllPlates();
            return false;
        }
    }

    private void ResetAllPlates()
    {
        foreach (PressurePlate plate in plates)
        {
            plate.ResetPlate();
        }
    }
}