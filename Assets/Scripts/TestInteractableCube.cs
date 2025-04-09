using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractableCube : MonoBehaviour, IInteractable
{
    [SerializeField] bool isInteractable;
    public void Interact()
    {
        RemoveFromNecessaryInteractables();
        Debug.Log("Interacted with " + gameObject.name);
    }

    public bool IsInteractable()
    {
        return true;
    }

    public void RemoveFromNecessaryInteractables()
    {
        GameplayController.Get().GetCurrentZone().RemoveFromNecessaryInteractions(gameObject);
    }
}
