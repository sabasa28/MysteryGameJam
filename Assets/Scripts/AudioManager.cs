using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource ambienceSource;
    [SerializeField] AudioSource stepsSource;
    [SerializeField] AudioClip uiSelectClip;
    [SerializeField] AudioClip uiBackClip;
    [SerializeField] AudioClip photoSound;
    [SerializeField] float ambienceVolumeSceneModifier = 1.0f;
    static AudioManager instance;

    public static AudioManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        audioSource.volume = SettingsData.volume;
        ambienceSource.volume = SettingsData.volume * ambienceVolumeSceneModifier * 0.5f;
        stepsSource.volume = SettingsData.volume;
        stepsSource.loop = true;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void UpdateVolume()
    {
        audioSource.volume = SettingsData.volume;
        ambienceSource.volume = SettingsData.volume * 0.5f;
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        audioSource.PlayOneShot(clip, volume * SettingsData.volume);
    }

    public void PlayUISelect()
    {
        audioSource.PlayOneShot(uiSelectClip, 1.0f * SettingsData.volume);
    }

    public void PlayUIBack()
    {
        audioSource.PlayOneShot(uiBackClip, 1.0f * SettingsData.volume);
    }

    public void PlaySteps(AudioClip clip, float pitch, float volume)
    {
        stepsSource.Stop();
        stepsSource.clip = clip;
        stepsSource.pitch = pitch;
        stepsSource.volume = SettingsData.volume * volume;
        stepsSource.Play();
    }

    public void StopSteps()
    {
        stepsSource.Stop();
    }

    public void PlayPhotoSound()
    {
        PlaySFX(photoSound);
    }
}
