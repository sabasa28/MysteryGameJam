using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightBeginsFailing : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().StartBreakingFlashlight();
    }
}
