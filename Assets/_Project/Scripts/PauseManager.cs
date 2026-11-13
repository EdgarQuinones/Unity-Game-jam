using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    public static bool IsPaused { get; private set; }
    private bool leaving;
    private float previousTimeScale = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPauseState()
    {
        IsPaused = false;
    }

    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        else
            Debug.LogWarning("Assign the Pause panel to PauseManager.", this);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (IsPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        if (leaving || pausePanel == null)
            return;

        previousTimeScale = Time.timeScale;
        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = previousTimeScale;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        if (leaving)
            return;

        leaving = true;
        Resume();
        Time.timeScale = 1f;
        RunManager.ClearPendingFortune();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        Resume();
    }
}