using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGManualTrigger : MonoBehaviour
{
    [SerializeField] bool trigger = false;
    [SerializeField] EventTriggerBase eventToTrigger;
    void Update()
    {
        if (trigger && eventToTrigger)
        {
            trigger = false;
            eventToTrigger.TriggerEvent();
        }
    }
}
