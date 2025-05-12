using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnDocument : DocumentHolder
{
    public override bool IsInteractable()
    {
        return LevelsManager.Get().persistentData.isReturning;
    }
}
