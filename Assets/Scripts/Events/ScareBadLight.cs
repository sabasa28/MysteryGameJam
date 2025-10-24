using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareBadLight : EventTriggerBase
{
    [SerializeField] Animator badLightAnimator;
    [SerializeField] GameObject invisibleWall;
    public override void TriggerEvent()
    {
        badLightAnimator.SetTrigger("Run");
        GameplayController.Get().ReenablePlayerFlashlight();
        invisibleWall.SetActive(false);
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
