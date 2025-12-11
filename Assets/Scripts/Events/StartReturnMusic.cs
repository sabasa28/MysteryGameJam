using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartReturnMusic : EventTriggerBase
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float defaultVolume;
    bool playing = false;

    private void Update()
    {
        if (playing)
        {
            audioSource.volume = defaultVolume * SettingsData.volume; //podria ser mas clean, no me importa tanto
        }
    }

    public override void TriggerEvent()
    {
        if (!playing)
        {
            playing = true;
            audioSource.Play();
        }
    }

    public void StopLooping()
    {
        if (playing)
        {
            audioSource.loop = false;
        }
    }
}
