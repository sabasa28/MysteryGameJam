using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
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
}
