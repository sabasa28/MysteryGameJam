using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTrigger : MonoBehaviour
{
    [SerializeField] LogEntry logEntryToAdd;
    [SerializeField] EventTriggerBase eventToTrigger;
    [SerializeField] float timeBeforeTriggering = 0.0f;
    bool waitingToTrigger = false;
    private void Start()
    {
        PersistentData persistentData = LevelsManager.Get().persistentData;
        if (persistentData.WasLogTriggered(logEntryToAdd))
        {
            gameObject.SetActive(false);
        }
        persistentData.AddLogToExisting(logEntryToAdd);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (waitingToTrigger)
        {
            return;
        }
        if (timeBeforeTriggering > 0)
        {
            StartCoroutine(WaitAndTrigger());
        }
        else
        {
            Trigger();
        }
    }
    IEnumerator WaitAndTrigger()
    {
        waitingToTrigger = true;
        yield return new WaitForSeconds(timeBeforeTriggering);
        Trigger();
    }

    void Trigger()
    {
        DocumentManager.Get().AddLogToFoundLogs(logEntryToAdd);
        AudioManager.Get().PlayPhotoSound();
        if (eventToTrigger != null)
        {
            eventToTrigger.TriggerEvent();
        }
        gameObject.SetActive(false);
    }
}
