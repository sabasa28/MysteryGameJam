using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanEndGameEvent : EventTriggerBase
{
    [SerializeField] Transform newExitDoor;
    [SerializeField] TextEntry endGameEnabledChat;
    public override void TriggerEvent()
    {
        LevelsManager.Get().persistentData.canEndGame = true;
        GameplayController.Get().GetCurrentZone().exitDoor = newExitDoor;
        ChatManager.Get().StartDisplayingTextEntry(endGameEnabledChat);
    }
}
