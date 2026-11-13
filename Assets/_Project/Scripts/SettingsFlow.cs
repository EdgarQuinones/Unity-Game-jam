using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsFlow : MonoBehaviour
{
    private bool leaving;

    public void ReturnToMainMenu()
    {
        if (leaving)
            return;

        leaving = true;
        SceneManager.LoadScene("MainMenu");
    }
}