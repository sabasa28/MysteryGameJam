using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyShowIfWeb : MonoBehaviour
{
    [SerializeField] bool todayIsOpossiteDay = false;
    void Start()
    {
        bool shouldHide = Application.platform != RuntimePlatform.WebGLPlayer;
        if (todayIsOpossiteDay)
        {
            shouldHide = !shouldHide;
        }
        if (shouldHide)
        {
            gameObject.SetActive(false);
        }
    }
}
