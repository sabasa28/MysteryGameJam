using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCircuit : MonoBehaviour
{
    [SerializeField] ParticleSystem sparksPS;
    [SerializeField] AudioSource sparkleazo;
    [SerializeField] AudioClip[] sparkleazoClips;
    public void EmitSparks()
    {
        sparksPS.Play();
        int i = Random.Range(0, sparkleazoClips.Length);
        sparkleazo.clip = sparkleazoClips[i];
        sparkleazo.Play();
    }
}
