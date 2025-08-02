using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSonarEvent : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().ForcePlayerSonar();
    }

}
