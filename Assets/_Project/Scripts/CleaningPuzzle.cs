using UnityEngine;

public abstract class CleaningPuzzle : MonoBehaviour
{
    public abstract void CompleteFromLuckyReel();

    protected void ReportCompletion()
    {
        GameManager manager = FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            Debug.LogError($"{name}: cannot report completion without a GameManager.", this);
            return;
        }

        manager.PuzzleCompleted(this);
    }
}