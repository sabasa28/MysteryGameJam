using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideStepsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().ForceDirtSteps(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().ForceDirtSteps(false);
        }
    }
}
