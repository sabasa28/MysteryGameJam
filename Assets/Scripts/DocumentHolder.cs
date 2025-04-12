using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentHolder : MonoBehaviour, IInteractable
{
    [SerializeField] Document document;

    public void Interact()
    {
        RemoveFromNecessaryInteractables();
        DocumentManager.Get().AddDocumentWordsToLearnable(document);
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
