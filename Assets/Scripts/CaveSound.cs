using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] posibleClips;
    [SerializeField]float minTimeBetweenSounds;
    [SerializeField]float maxTimeBetweenSounds;
    float initialVolume;
    float timer = 0.0f;
    float timeForNextSound;
    int nextSoundIndex;
    private void Start()
    {
        initialVolume = audioSource.volume;
        nextSoundIndex = Random.Range(0,posibleClips.Length);
        timeForNextSound = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeForNextSound)
        {
            timer = 0.0f;
            audioSource.volume = initialVolume * SettingsData.volume;
            audioSource.PlayOneShot(posibleClips[nextSoundIndex]);
            nextSoundIndex = Random.Range(0, posibleClips.Length);
            timeForNextSound = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
        }
    }
}
