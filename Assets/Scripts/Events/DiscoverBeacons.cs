using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverBeacons : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().DiscoverBeacons();
    }
}
