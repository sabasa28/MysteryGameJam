using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverSecrets : EventTriggerBase
{
    public override void TriggerEvent()
    {
        DocumentManager.Get().LearnSecrets();
    }
}
