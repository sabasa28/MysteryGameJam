using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixFlashlight : EventTriggerBase
{
    [SerializeField] float timeBeforeFixing;
    public override void TriggerEvent()
    {
        GameplayController.Get().ReenablePlayerFlashlight(timeBeforeFixing);
    }
}
