using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    static UIGameplay instance;
    [SerializeField] GameObject InteractText;
    public static UIGameplay Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void ChangeInteractTextDisplay(bool bDisplay)
    {
        InteractText.SetActive(bDisplay);
    }
}
