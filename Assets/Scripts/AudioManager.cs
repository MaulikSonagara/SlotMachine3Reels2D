using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource uiSFXSource;       // for buttons, win/lose, etc.
    [SerializeField] AudioSource[] reelSources;     // one source per reel

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip iconSelect;
    public AudioClip reelSpin;

    public AudioClip betAmountPlusSFX;
    public AudioClip betAmountMinusSFX;

    public AudioClip winAudio2X;
    public AudioClip winAudio3X;
    public AudioClip looseAudio;

    // PlayerPrefs keys
    const string KEY_MUSIC_VOL = "SM_MusicVol";
    const string KEY_SFX_VOL = "SM_SFXVol";

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;

        // Load saved volumes
        float musicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, 1f);
        float sfxVol = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);

        musicSource.volume = musicVol;
        uiSFXSource.volume = sfxVol;

        // Apply to all reels too
        foreach (var src in reelSources)
        {
            if (src != null) src.volume = sfxVol;
        }

        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            uiSFXSource.PlayOneShot(clip);
    }

   
    public void PlayReelSpin(int reelIndex)
    {
        if (reelIndex < 0 || reelIndex >= reelSources.Length) return;

        var src = reelSources[reelIndex];
        src.clip = reelSpin;
        src.loop = true;
        src.pitch = Random.Range(0.95f, 1.05f); 
        src.Play();
    }

    public void StopReelSpin(int reelIndex)
    {
        if (reelIndex < 0 || reelIndex >= reelSources.Length) return;

        var src = reelSources[reelIndex];
        src.Stop();
        src.loop = false;
        src.clip = null;

        uiSFXSource.pitch = Random.Range(0.95f, 1.05f);
        uiSFXSource.PlayOneShot(iconSelect);
        uiSFXSource.pitch = 1f;
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        uiSFXSource.volume = value;
        foreach (var src in reelSources)
        {
            if (src != null) src.volume = value;
        }

        PlayerPrefs.SetFloat(KEY_SFX_VOL, value);
        PlayerPrefs.Save();
    }

    // Getter helpers
    public float GetMusicVolume() => musicSource.volume;
    public float GetSFXVolume() => uiSFXSource.volume;
}
