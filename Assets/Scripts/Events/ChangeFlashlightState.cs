using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFlashlightState : EventTriggerBase
{
    [SerializeField] bool stateToChangeTo;
    public override void TriggerEvent()
    {
        GameplayController.Get().ForcePlayerFlashlight(stateToChangeTo);
    }

}
