using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverDates : EventTriggerBase
{
    public override void TriggerEvent()
    {
        GameplayController.Get().DiscoverCalendar();
    }
}
