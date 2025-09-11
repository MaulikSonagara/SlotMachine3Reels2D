using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    [Header("UI References")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Text musicPercentText;
    public Text sfxPercentText;
    public Button BackBtn;

    private void Start()
    {
        // Initialize with saved values
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        UpdateMusicText(musicSlider.value);
        UpdateSFXText(sfxSlider.value);

        // Add listeners
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

    }


    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        UpdateMusicText(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        UpdateSFXText(value);
    }

    private void UpdateMusicText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        musicPercentText.text = percent + "%";
    }

    private void UpdateSFXText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        sfxPercentText.text = percent + "%";
    }
}
