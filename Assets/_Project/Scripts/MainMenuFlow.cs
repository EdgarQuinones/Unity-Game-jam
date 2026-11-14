using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuFlow : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private bool leaving;

    private void Awake()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (leaving || settingsPanel == null || !settingsPanel.activeInHierarchy)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseSettings();
    }

    public void StartGame()
    {
        if (leaving)
            return;

        leaving = true;
        SceneManager.LoadScene("OutsideTemple");
    }

    public void OpenSettings()
    {
        if (leaving)
            return;

        if (settingsPanel == null)
        {
            Debug.LogWarning("Assign the Settings panel to MainMenuFlow.", this);
            return;
        }

        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}