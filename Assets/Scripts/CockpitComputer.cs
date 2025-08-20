using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CockpitComputer : MonoBehaviour, IInteractable
{
    [SerializeField] Transform InteractingCameraPos;
    [SerializeField] float timeMovingCamera;
    [SerializeField] float timeReturningCamera;
    bool isBeingLookedAt = false;
    bool onInitialScreen = true;
    [SerializeField] GameObject newMessagesPanel;
    [SerializeField] GameObject listOfMessagesPanel;
    [SerializeField] GameObject message1Panel;
    [SerializeField] GameObject message2Panel;
    [SerializeField] Button message2Button;
    ColorBlock message2ButtonColorBlockAux;
    bool message1Read = false;
    bool message2Read = false;

    private void Start()
    {
        ColorBlock auxColorBlock = message2Button.colors;
        message2ButtonColorBlockAux = auxColorBlock;
        auxColorBlock.highlightedColor = auxColorBlock.normalColor;
        auxColorBlock.pressedColor = auxColorBlock.normalColor;
        message2Button.colors = auxColorBlock;
    }
    public void Interact()
    {
        GameplayController.Get().MovePlayerCameraAndReturn(InteractingCameraPos, timeMovingCamera, timeReturningCamera, GameplayController.InputState.UI);
        StartCoroutine(WaitForZoomInAndTakeInput());
    }

    public bool IsInteractable()
    {
        return (!message1Read || !message2Read);// && LevelsManager.Get().persistentData.canEndGame;
    }

    public void RemoveFromNecessaryInteractables()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (isBeingLookedAt)
        {
            if (onInitialScreen && Input.GetKeyDown(KeyCode.Mouse0))
            {
                onInitialScreen = false;
                OpenListOfMessages();   
            }
        }
    }

    public void StopBeingLookedAt()
    {
        GameplayController.Get().ReturnPlayerCamera();
        isBeingLookedAt = false;
    }

    IEnumerator WaitForZoomInAndTakeInput()
    {
        yield return new WaitForSeconds(timeMovingCamera);
        isBeingLookedAt = true;
    }

    public void OpenListOfMessages()
    {
        newMessagesPanel.SetActive(false);
        listOfMessagesPanel.SetActive(true);
        message1Panel.SetActive(false);
        message2Panel.SetActive(false);
    }
    public void OpenMessage1()
    {
        message1Read = true;
        newMessagesPanel.SetActive(false);
        listOfMessagesPanel.SetActive(false);
        message1Panel.SetActive(true);
        message2Panel.SetActive(false);
        message2Button.colors = message2ButtonColorBlockAux;
    }
    public void OpenMessage2()
    {
        if (!message1Read)
        {
            return;
        }
        message2Read = true;
        newMessagesPanel.SetActive(false);
        listOfMessagesPanel.SetActive(false);
        message1Panel.SetActive(false);
        message2Panel.SetActive(true);
    }

    public void CloseCockpitcomputer()
    {
        if (message1Read && message2Read)
        {
            GameplayController gpc = GameplayController.Get();
            StopBeingLookedAt();
            gpc.ChangeInputState(GameplayController.InputState.UI);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gpc.StartEndGameCinematic();
        }
    }
}
