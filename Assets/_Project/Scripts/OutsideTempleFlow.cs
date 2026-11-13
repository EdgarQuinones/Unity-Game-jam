using UnityEngine;
using UnityEngine.SceneManagement;

public class OutsideTempleFlow : MonoBehaviour
{
    private bool leaving;

    private void Start()
    {
        RunManager.PrepareFortune(out int cleaning, out int encounter, out int boss);
        int puzzles = cleaning <= 3 ? 3 : cleaning <= 6 ? 2 : 1;
        int enemies = encounter <= 3 ? 4 : encounter <= 6 ? 3 : 2;
        string bossPacing = boss <= 3 ? "faster" : boss <= 6 ? "normal" : "slower";
        Debug.Log($"Opening Fortune: Cleaning {cleaning} ({puzzles} puzzles), Encounter {encounter} ({enemies} enemies), Boss {boss} ({bossPacing} attacks).", this);
    }

    public void ContinueToTemple()
    {
        if (leaving)
            return;

        leaving = true;
        SceneManager.LoadScene("InsideTemple");
    }

    public void ReturnToMainMenu()
    {
        if (leaving)
            return;

        leaving = true;
        RunManager.ClearPendingFortune();
        SceneManager.LoadScene("MainMenu");
    }
}