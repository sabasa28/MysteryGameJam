using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverNames : EventTriggerBase
{
    public override void TriggerEvent()
    {
        DocumentManager.Get().LearnNames();
    }
}
