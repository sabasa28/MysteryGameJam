using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCircuit : MonoBehaviour
{
    [SerializeField] ParticleSystem sparksPS;
    public void EmitSparks()
    {
        sparksPS.Play();
    }
}
