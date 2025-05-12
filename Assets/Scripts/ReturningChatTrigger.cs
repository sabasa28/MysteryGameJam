using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningChatTrigger : ChatTrigger
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (!LevelsManager.Get().persistentData.isReturning)
        {
            return;
        }
        base.OnTriggerEnter(other);
    }
}
