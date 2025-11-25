using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSourceVolumePeriodically : MonoBehaviour
{
    float originalValue;
    AudioSource audioSource;
    float timer = 0.0f;
    float refreshTime = 0.5f;
    void Start()
    {
        audioSource = GetComponent <AudioSource>();
        originalValue = audioSource.volume;
        audioSource.volume = originalValue * SettingsData.volume;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > refreshTime)
        {
            timer = 0.0f;
            audioSource.volume = originalValue * SettingsData.volume;
        }
    }
}
