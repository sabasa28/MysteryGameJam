using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpToFromLab : MonoBehaviour
{
    [SerializeField]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameplayController.Get().IsZoneDone())
            {
                GameplayController.Get().MovePlayerInOutOfLab();
            }
            else
            {
                ChatManager.Get().PlayNotDoneWithZoneChat();
            }
        }
    }
}
