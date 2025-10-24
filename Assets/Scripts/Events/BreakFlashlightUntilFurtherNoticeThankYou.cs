using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFlashlightUntilFurtherNoticeThankYou : EventTriggerBase
{
    [SerializeField] GameObject invisibleWall;
    public override void TriggerEvent()
    {
        if (invisibleWall)
        {
            invisibleWall.SetActive(true);
        }
        GameplayController.Get().FlickerAndDisablePlayerFlashlight();
        AudioManager.Get().UpdateBackgroundVolume(0.0f, true);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerEvent();
        }
    }
}
