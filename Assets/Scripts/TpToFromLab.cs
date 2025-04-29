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
            GameplayController.Get().MovePlayerInOutOfLab();
        }
    }
}
