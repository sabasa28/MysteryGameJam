using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartReturning : EventTriggerBase
{
    public override void TriggerEvent()
    {
        LevelsManager.Get().persistentData.isReturning = true;
    }

}
