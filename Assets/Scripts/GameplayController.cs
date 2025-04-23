using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameplayController : MonoBehaviour
{
    static GameplayController instance;

    public static GameplayController Get()
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
    public enum InputState
    {
        Chat,
        Movement,
        Cinematic,
        InGameUI
    }
    InputState inputState;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ChatManager chatManager;
    [SerializeField] ZoneData currentZone;
    [SerializeField] ZoneData[] zones;
    [SerializeField] Transform outsideOfShipPos;
    [SerializeField] Transform insideOfShipPos;
    [SerializeField] Volume helmetVolume;
    bool inGameMenuOpen = false;
    [SerializeField] SelectingFinger selectingFinger;
    bool isPlayerInShip = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !playerMovement.isAnimatingUiHand)
        {
            inGameMenuOpen = !inGameMenuOpen;
            ChangeInputState(inGameMenuOpen ? InputState.Chat : InputState.Movement);
            selectingFinger.gameObject.SetActive(inGameMenuOpen);

            if (inGameMenuOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (inGameMenuOpen)
            {
                playerMovement.RaiseHand();
            }
            else
            { 
                playerMovement.LowerHand();
            }
        }
    }
    public void ChangeInputState(InputState newState)
    {
        inputState = newState;
        switch (inputState)
        {
            case InputState.Chat:
                playerMovement.SetInputState(false);
                //we don't need to turn on text mode since it will turn on by itself when we send it a text entry
                break;
            case InputState.Movement:
                playerMovement.SetInputState(true);
                chatManager.TurnOffTextMode();
                break;
            case InputState.Cinematic:
                playerMovement.SetInputState(false);
                chatManager.TurnOffTextMode();
                break;
            case InputState.InGameUI:
                playerMovement.SetInputState(false);
                chatManager.TurnOffTextMode();

                break;
            default:
                break;
        }
    }

    public ZoneData GetCurrentZone()
    {
        return currentZone;
    }

    public void SpawnMapBeacons()
    {
        playerMovement.SpawnPersistentBeacons();
    }

    public void MovePlayerInOutOfShip()
    {
        isPlayerInShip = !isPlayerInShip;
        StartCoroutine(MovePlayerInOutShipCoroutine(isPlayerInShip));
    }

    IEnumerator MovePlayerInOutShipCoroutine(bool moveIn)
    {
        ChangeInputState(InputState.Cinematic);
        UIGameplay.Get().FadeOutAndIn();
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        playerMovement.GetComponent<CharacterController>().enabled = false;
        playerMovement.transform.position = moveIn? insideOfShipPos.position : outsideOfShipPos.position;
        playerMovement.GetComponent<CharacterController>().enabled = true;
        helmetVolume.enabled = !moveIn;
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingIn);
        ChangeInputState(InputState.Movement);
    }

}
