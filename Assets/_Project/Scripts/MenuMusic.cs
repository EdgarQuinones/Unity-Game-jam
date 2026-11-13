using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusic : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private static MenuMusic instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Start()
    {
        if (instance != this)
            return;

        AudioSettings.ApplySavedVolumes(audioMixer);
        GetComponent<AudioSource>().Play();
    }

    private void OnActiveSceneChanged(Scene previous, Scene current)
    {
        if (current.name != "MainMenu" && current.name != "Settings")
        {
            GetComponent<AudioSource>().Stop();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        instance = null;
    }
}