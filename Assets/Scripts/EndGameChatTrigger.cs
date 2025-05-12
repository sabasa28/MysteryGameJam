using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameChatTrigger : ChatTrigger
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (!LevelsManager.Get().persistentData.canEndGame)
        {
            return;
        }
        base.OnTriggerEnter(other);
    }
}
