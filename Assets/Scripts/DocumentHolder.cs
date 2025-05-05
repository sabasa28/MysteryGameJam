using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentHolder : MonoBehaviour, IInteractable
{
    [SerializeField] Document document;

    private void Start()
    {
        if (LevelsManager.Get().persistentData.WasDocFound(document))
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        RemoveFromNecessaryInteractables();
        DocumentManager.Get().AddDocumentWordsToLearnable(document);
        gameObject.SetActive(false);
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
