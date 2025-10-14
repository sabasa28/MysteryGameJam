using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GeneralBrightnessSettings : MonoBehaviour
{
    static GeneralBrightnessSettings instance;

    public static GeneralBrightnessSettings Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    LiftGammaGain liftGammaGain = null;
    Vector4 currentGain;
    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        if (liftGammaGain)
        {
            return;
        }

        Volume volumeComp = GetComponent<Volume>();
        if (volumeComp && volumeComp.profile)
        {
            volumeComp.enabled = true;
            volumeComp.profile.TryGet(out liftGammaGain);

        }

        if (liftGammaGain)
        {
            currentGain = liftGammaGain.gain.value;
        }
    }

    public void UpdateGain(float newGain)
    {
        if (liftGammaGain)
        {
            currentGain.w = newGain;
            liftGammaGain.gain.Override(currentGain);
        }
    }

    public float GetGain()
    {
        TryInitialize();
        if (liftGammaGain)
        {
            return currentGain.w;
        }
        else
        {
            Debug.Log("ERROR RETORNANDO GAIN");
            return -1.0f;
        }
    }
}
