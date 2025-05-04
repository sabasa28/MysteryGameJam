using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverHook : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().DiscoverHook();
    }
}
