using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGLogsAndDocs : MonoBehaviour, IInteractable
{
    bool grabed = false;
    [SerializeField] Document[] docsToObtain;
    [SerializeField] LogEntry[] logsToObtain;
    public void Interact()
    {
        for (int i = 0; i < docsToObtain.Length; i++)
        {
            DocumentManager.Get().AddDocumentWordsToLearnable(docsToObtain[i]);
        }
        for (int i = 0; i < logsToObtain.Length; i++)
        {
            DocumentManager.Get().AddLogToFoundLogs(logsToObtain[i]);
        }

        grabed = true;
    }

    public bool IsInteractable()
    {
        return !grabed;
    }

    public void RemoveFromNecessaryInteractables()
    {
    }

}
