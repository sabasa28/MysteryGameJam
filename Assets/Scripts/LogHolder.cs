using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHolder : MonoBehaviour, IInteractable
{
    [SerializeField] LogEntry log;

    public void Interact()
    {
        RemoveFromNecessaryInteractables();
        DocumentManager.Get().AddLogToFoundLogs(log);
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
