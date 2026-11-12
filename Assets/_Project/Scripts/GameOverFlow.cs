    using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFlow : MonoBehaviour
{
    public enum RunResult { Unknown, Victory, Defeat }

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    private static RunResult pendingResult;
    private bool leaving;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetResult()
    {
        pendingResult = RunResult.Unknown;
    }

    public static void SetResult(RunResult result)
    {
        pendingResult = result;
    }

    private void Start()
    {
        RunResult result = pendingResult;
        pendingResult = RunResult.Unknown;

        if (victoryPanel != null)
        {
            //Debug.Log("Victory panel!");
            victoryPanel.SetActive(result == RunResult.Victory);
        }
        if (defeatPanel != null)
        {
            //Debug.Log("Defeat panel!");
            defeatPanel.SetActive(result == RunResult.Defeat);
        }

        if (victoryPanel == null || defeatPanel == null)
            Debug.LogWarning("Assign both Victory and Defeat panels to GameOverFlow.", this);
        if (result == RunResult.Unknown)
            Debug.LogWarning("No run result was provided; start from InsideTemple to test an ending.", this);
    }

    public void PlayAgain()
    {
        if (leaving)
            return;

        leaving = true;
        RunManager.ClearPendingFortune();
        SceneManager.LoadScene("OutsideTemple");
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