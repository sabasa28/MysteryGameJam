using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverSonar : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().DiscoverSonar();
    }
}
