using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningChatTrigger : ChatTrigger
{
    [SerializeField] GameObject FakewallToActivate;
    protected override void Start()
    {
        if (LevelsManager.Get().persistentData.WasChatPlayed(textEntryToDisplay) && LevelsManager.Get().persistentData.isReturning)
        {
            eventToTrigger.TriggerEvent();
        }
        base.Start();
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!LevelsManager.Get().persistentData.isReturning)
        {
            return;
        }
        if (FakewallToActivate)
        {
            FakewallToActivate.SetActive(true);
        }
        base.OnTriggerEnter(other);
    }
}
