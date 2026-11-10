using UnityEngine;

public class PressurePlatePuzzle : CleaningPuzzle
{
    private int[] correctSequence;

    [SerializeField] private PressurePlate[] plates;

    private CatHappiness catHappiness;

    private int currentStep = 0;

    private bool isCompleted = false;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        catHappiness = FindFirstObjectByType<CatHappiness>();
        GenerateSequence();
    }

    private void GenerateSequence()
    {
        if (plates == null || plates.Length == 0)
        {
            Debug.LogError($"{name}: no pressure plates assigned; puzzle cannot be solved.", this);
            return;
        }

        correctSequence = new int[plates.Length];
        for (int i = 0; i < plates.Length; i++)
        {
            if (plates[i] == null)
            {
                Debug.LogError($"{name}: pressure plate {i} is missing; puzzle cannot be solved.", this);
                correctSequence = null;
                return;
            }

            correctSequence[i] = plates[i].PlateID;
            for (int j = 0; j < i; j++)
            {
                if (correctSequence[j] == correctSequence[i])
                {
                    Debug.LogError($"{name}: duplicate plate ID {correctSequence[i]}; puzzle cannot be solved.", this);
                    correctSequence = null;
                    return;
                }
            }
        }

        for (int i = correctSequence.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temporary = correctSequence[i];
            correctSequence[i] = correctSequence[randomIndex];
            correctSequence[randomIndex] = temporary;
        }

        //Debug.Log($"{name}: generated plate sequence: {string.Join(" -> ", correctSequence)}", this);
    }

    public bool ActivatePlate(int plateID)
    {
        if (correctSequence == null || isCompleted ||
            gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return false;

        if (plateID == correctSequence[currentStep])
        {
            Debug.Log($"{name}: correct plate {plateID}.");
            currentStep++;


            if (currentStep >= correctSequence.Length)
            {
                CompletePuzzle();
            }

            return true;
        }
        else
        {
            currentStep = 0;

            Debug.Log($"{name}: incorrect plate {plateID}; sequence reset.");

            catHappiness.ModifyHappiness(-5);

            ResetAllPlates();
            return false;
        }
    }

    public override void CompleteFromLuckyReel()
    {
        if (isCompleted || gameManager == null ||
            gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return;

        Debug.Log($"{name}: completed by Lucky Reel.", this);
        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (isCompleted)
            return;

        isCompleted = true;
        Debug.Log($"{name} completed!");
        catHappiness.ModifyHappiness(10);
        ReportCompletion();
    }

    private void ResetAllPlates()
    {
        foreach (PressurePlate plate in plates)
        {
            plate.ResetPlate();
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isCompleted || plates == null)
            return;

        Gizmos.color = Color.green;
        foreach (PressurePlate plate in plates)
        {
            if (plate != null)
                Gizmos.DrawWireSphere(plate.transform.position, 0.65f);
        }
    }
}