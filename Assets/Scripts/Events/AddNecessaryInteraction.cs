using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNecessaryInteraction : EventTriggerBase
{
    [SerializeField] GameObject interactable;
    public override void TriggerEvent()
    {
        GameplayController.Get().GetCurrentZone().AddNecessaryInteraction(interactable);
    }
}
