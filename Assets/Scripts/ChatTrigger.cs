using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    [SerializeField] TextEntry textEntryToDisplay;
    [SerializeField] EventTriggerBase eventToTrigger;
    [SerializeField] float timeBeforeTriggering = 0.0f;
    bool waitingToTrigger = false;
    private void Start()
    {
        if (LevelsManager.Get().persistentData.WasChatPlayed(textEntryToDisplay))
        {
            gameObject.SetActive(false);
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
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
        ChatManager.Get().StartDisplayingTextEntry(textEntryToDisplay);
        if (eventToTrigger != null)
        {
            eventToTrigger.TriggerEvent();
        }
        LevelsManager.Get().persistentData.AddChatToPlayedChats(textEntryToDisplay);
        gameObject.SetActive(false);
    }
}
