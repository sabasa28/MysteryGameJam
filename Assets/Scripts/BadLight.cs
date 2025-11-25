using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadLight : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public void PlayDroneSound()
    {
        audioSource.volume *= SettingsData.volume;
        audioSource.Play();
    }
}
