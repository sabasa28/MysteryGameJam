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
        ambienceSource.volume = SettingsData.volume * ambienceVolumeSceneModifier;
        stepsSource.volume = SettingsData.volume;
        stepsSource.loop = true;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void UpdateVolume()
    {
        audioSource.volume = SettingsData.volume;
        ambienceSource.volume = SettingsData.volume * ambienceVolumeSceneModifier;
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

    public void PlaySteps(AudioClip clip, float pitch, float volume) //we asume loop is true
    {
        stepsSource.Stop();
        stepsSource.clip = clip;
        stepsSource.pitch = pitch;
        stepsSource.volume = SettingsData.volume * volume;
        stepsSource.Play();
    }

    public void PlayIndividualStep(AudioClip clip, float pitch, float volume)
    {
        stepsSource.Stop();
        stepsSource.loop = false;
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

    public void UpdateBackgroundVolume(float ambienceVol, bool fade)
    {
        if (fade)
        {
            StartCoroutine(LerpBackgroundVol(ambienceVolumeSceneModifier, ambienceVol));
        }
        else
        { 
            ambienceVolumeSceneModifier = ambienceVol;
            UpdateVolume();
        }
    }

    IEnumerator LerpBackgroundVol(float initialAmbienceVol, float targetAmbienceVol)
    {
        float lerpTime = 1.0f;
        float timer = 0.0f;
        while (timer < lerpTime)
        {
            ambienceVolumeSceneModifier = Mathf.Lerp(initialAmbienceVol, targetAmbienceVol, timer / lerpTime);
            UpdateVolume();
            timer += Time.deltaTime;
            yield return null;
        }
        ambienceVolumeSceneModifier = targetAmbienceVol;
        UpdateVolume();
    }
}
