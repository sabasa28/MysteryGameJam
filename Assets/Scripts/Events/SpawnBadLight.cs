using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBadLight : EventTriggerBase
{
    [SerializeField] GameObject badLight;
    [SerializeField] GameObject scareBadLightTrigger;
    [SerializeField] GameObject breakFlashlightTrigger;

    public override void TriggerEvent()
    {
        badLight.SetActive(true);
        scareBadLightTrigger.SetActive(true);
        breakFlashlightTrigger.SetActive(true);
    }

}
