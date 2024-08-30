using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] public AudioSource bgmSource;
    [SerializeField] public List<AudioSource> sfxSources = new List<AudioSource>();

    private float bgmVolume = 1.0f;
    private float sfxVolume = 1.0f;
    private float masterVolume = 1.0f;
    private bool isPaused = false;
    
    protected override void Awake()
    {
        base.Awake();
        if (bgmSource == null)
        {
            Debug.LogError("BGM AudioSource is not assigned in the inspector.");
        }
        if (sfxSources.Count == 0)
        {
            Debug.LogError("SFX AudioSources are not assigned in the inspector.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           // UiUtils.GetUI<SettingUI>().gameObject.SetActive(!UiUtils.GetUI<SettingUI>().gameObject.activeSelf);
            TogglePause();
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 1.0f)
    {
        if (bgmSource != null)
        {
            bgmSource.clip = clip;
            bgmVolume = volume;
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning("No available SFX source to play the clip.");
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
    }
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
        // SFX는 PlaySFX 메서드 호출 시 볼륨이 반영됨
    }
    public float GetBGMVolume()
    {
        return bgmVolume;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
