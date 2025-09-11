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

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    // UI / one-shot SFX
    public void PlaySFX(AudioClip clip)
    {
        uiSFXSource.PlayOneShot(clip);
    }

    // Reel loop play
    public void PlayReelSpin(int reelIndex)
    {
        if (reelIndex < 0 || reelIndex >= reelSources.Length) return;

        var src = reelSources[reelIndex];
        src.clip = reelSpin;
        src.loop = true;
        src.pitch = Random.Range(0.95f, 1.05f); // slight variation
        src.Play();
    }

    // Reel stop
    public void StopReelSpin(int reelIndex)
    {
        if (reelIndex < 0 || reelIndex >= reelSources.Length) return;

        var src = reelSources[reelIndex];
        src.Stop();
        src.loop = false;
        src.clip = null;

        // play icon select on stop
        uiSFXSource.pitch = Random.Range(0.95f, 1.05f);
        uiSFXSource.PlayOneShot(iconSelect);
        uiSFXSource.pitch = 1f;
    }
}
