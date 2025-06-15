using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentHolder : MonoBehaviour, IInteractable
{
    [SerializeField] Document document;
    [SerializeField] EventTriggerBase eventToTrigger;

    private void Start()
    {
        PersistentData persistentData = LevelsManager.Get().persistentData;
        if (persistentData.WasDocFound(document))
        {
            gameObject.SetActive(false);
        }
        persistentData.AddDocToExisting(document);
    }

    public void Interact()
    {
        RemoveFromNecessaryInteractables();
        DocumentManager.Get().AddDocumentWordsToLearnable(document);
        if (eventToTrigger != null)
        {
            eventToTrigger.TriggerEvent();
        }
        gameObject.SetActive(false);
    }

    public virtual bool IsInteractable()
    {
        return gameObject.activeInHierarchy && !LevelsManager.Get().persistentData.WasDocFound(document);
    }

    public void RemoveFromNecessaryInteractables()
    {
        GameplayController.Get().GetCurrentZone().RemoveFromNecessaryInteractions(gameObject);
    }
}
