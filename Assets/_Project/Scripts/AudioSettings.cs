using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MasterKey = "MasterVolume";
    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SFXVolume";

    private void Start()
    {
        ApplySavedVolume(MasterKey, masterSlider);
        ApplySavedVolume(MusicKey, musicSlider);
        ApplySavedVolume(SfxKey, sfxSlider);

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void OnDestroy()
    {
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        PlayerPrefs.Save();
    }

    private void ApplySavedVolume(string key, Slider slider)
    {
        float value = Mathf.Clamp01(PlayerPrefs.GetFloat(key, 1f));
        if (slider != null)
            slider.SetValueWithoutNotify(value);
        ApplyVolume(key, value);
    }

    public void SetMasterVolume(float value) => SaveVolume(MasterKey, value);
    public void SetMusicVolume(float value) => SaveVolume(MusicKey, value);
    public void SetSfxVolume(float value) => SaveVolume(SfxKey, value);

    private void SaveVolume(string key, float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(key, value);
        ApplyVolume(key, value);
    }

    private void ApplyVolume(string key, float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("Assign GameAudioMixer to AudioSettings.", this);
            return;
        }

        float decibels = value <= 0f ? -80f : Mathf.Log10(value) * 20f;
        if (!audioMixer.SetFloat(key, decibels))
            Debug.LogWarning($"Audio Mixer parameter '{key}' was not found.", this);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}