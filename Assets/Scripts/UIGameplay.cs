using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    static UIGameplay instance;
    [SerializeField] GameObject InteractText;
    [SerializeField] GameObject playerDocs;
    bool playerDocsActive;

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

    private void Start()
    {
        playerDocsActive = playerDocs.activeInHierarchy;
        SetPlayerDocsActiveState(playerDocsActive);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetPlayerDocsActiveState(!playerDocsActive);
        }
    }

    public bool IsCameraLocked()
    {
        return playerDocsActive;
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

    public void SetPlayerDocsActiveState(bool state)
    {
        playerDocsActive = state;
        if (playerDocsActive)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = playerDocsActive;
        playerDocs.SetActive(playerDocsActive);
    }
}
