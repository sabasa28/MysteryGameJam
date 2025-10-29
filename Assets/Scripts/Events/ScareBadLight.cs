using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareBadLight : EventTriggerBase
{
    [SerializeField] Animator badLightAnimator;
    [SerializeField] GameObject invisibleWall;
    [SerializeField] EventTriggerBase eventAfterDarknessChat;
    [SerializeField] BoxCollider triggerCollider;
    public override void TriggerEvent()
    {
        badLightAnimator.SetTrigger("Run");
        StartCoroutine(ChatInDarknessAndReenableFlashlight());
        invisibleWall.SetActive(false);
        triggerCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerEvent();
        }
    }

    IEnumerator ChatInDarknessAndReenableFlashlight()
    {
        yield return new WaitForSeconds(5);
        ChatManager.Get().PlayDarknessChat(eventAfterDarknessChat);
    }
}
