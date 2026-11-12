using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFlow : MonoBehaviour
{
    private bool leaving;

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

        leaving = true;
        SceneManager.LoadScene("Settings");
    }
}