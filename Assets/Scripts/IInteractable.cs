using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool AttemptInteract()
    {
        bool bInteracted = IsInteractable();
        if (bInteracted)
        {
            Interact();
        }
        return bInteracted;
    }
    void Interact();
    bool IsInteractable();
    void RemoveFromNecessaryInteractables();

}
