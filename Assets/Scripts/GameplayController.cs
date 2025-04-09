using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Cinematic
    }
    InputState inputState;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ChatManager chatManager;
    [SerializeField] ZoneData currentZone;
    [SerializeField] ZoneData[] zones;
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
            default:
                break;
        }
    }

    public ZoneData GetCurrentZone()
    {
        return currentZone;
    }
}
