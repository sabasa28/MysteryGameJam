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
        InGameUI,
        UI
    }
    InputState inputState;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ChatManager chatManager;
    [SerializeField] ZoneData currentZone;
    ZoneData initialZone;
    [SerializeField] ZoneData labZone;
    [SerializeField] Transform outsideOfShipPos;
    [SerializeField] Transform insideOfShipPos;
    [SerializeField] Transform outsideOfLabPos;
    [SerializeField] Transform insideOfLabPos;
    [SerializeField] Volume helmetVolume;
    bool inGameMenuOpen = false;
    bool optionsMenuOpen = false;
    [SerializeField] SelectingFinger selectingFinger;
    bool isPlayerInShip = true;
    bool isPlayerInLab = false;
    InputState savedInputState;
    [SerializeField] float lightScalar;
    [SerializeField] float lightAttenuationExponent;
    [SerializeField] GameObject playerDocs;
    bool playerDocsActive;

    private void Start()
    {
        Shader.SetGlobalFloat("_LightScalar", lightScalar); // B)
        Shader.SetGlobalFloat("_LightAttenuationExponent", lightAttenuationExponent);
        ChangeInputState(InputState.Movement);
        playerDocsActive = playerDocs.activeInHierarchy;
        SetPlayerDocsActiveState(playerDocsActive);
        initialZone = currentZone;
        ChangeCurrentZone(currentZone); // not really changing just loading playerdata
    }
    private void Update()
    {

        if ((Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Tab)) && !playerMovement.isAnimatingUiHand && inputState != InputState.Chat && inputState != InputState.Cinematic)
        {
            inGameMenuOpen = !inGameMenuOpen;
            ChangeInputState(inGameMenuOpen ? InputState.InGameUI : InputState.Movement);

            if (inGameMenuOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnUIMenuStateChanged();
            UIGameplay.Get().ChangeMenuVisibility(optionsMenuOpen);
        }
    }
    public void OnUIMenuStateChanged()
    {
        optionsMenuOpen = !optionsMenuOpen;
        if (optionsMenuOpen)
        {
            savedInputState = inputState;
            ChangeInputState(InputState.UI);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ChangeInputState(savedInputState);
            switch (inputState)
            {
                case InputState.InGameUI:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    break;
                case InputState.Chat:
                case InputState.Movement:
                case InputState.Cinematic:
                default:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
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
            case InputState.UI:
                playerMovement.SetInputState(false);
                break;
            default:
                break;
        }
    }

    public bool IsCameraLocked()
    {
        return playerDocsActive;
    }

    public ZoneData GetCurrentZone()
    {
        return currentZone;
    }

    public void SpawnMapBeacons()
    {
        playerMovement.SpawnPersistentBeacons();
    }

    public void LoadPlayerPersistence()
    {
        playerMovement.LoadPersistentData();
    }

    public void LoadHelmetState(bool helmetState)
    {
        helmetVolume.enabled = helmetState;
        isPlayerInShip = !helmetState;
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
            Cursor.visible = false;
        }
        playerDocs.SetActive(playerDocsActive);
    }

    public void SetFingerActiveState(bool state)
    {
        selectingFinger.gameObject.SetActive(state);
    }

    public void MovePlayerInOutOfShip()
    {
        isPlayerInShip = !isPlayerInShip;
        StartCoroutine(MovePlayerInOutShipCoroutine(isPlayerInShip));
    }

    public void MovePlayerInOutOfLab()
    {
        isPlayerInLab = !isPlayerInLab;
        StartCoroutine(MovePlayerInOutLabCoroutine(isPlayerInLab));
    }

    IEnumerator MovePlayerInOutShipCoroutine(bool moveIn)
    {
        ChangeInputState(InputState.Cinematic);
        UIGameplay.Get().FadeOutAndIn();
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        playerMovement.GetComponent<CharacterController>().enabled = false;
        playerMovement.transform.position = moveIn ? insideOfShipPos.position : outsideOfShipPos.position;
        playerMovement.GetComponent<CharacterController>().enabled = true;
        helmetVolume.enabled = !moveIn;
        LevelsManager.Get().persistentData.UpdateHelmetState(helmetVolume.enabled);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingIn);
        ChangeInputState(InputState.Movement);
    }

    IEnumerator MovePlayerInOutLabCoroutine(bool moveIn)
    {
        ChangeInputState(InputState.Cinematic);
        UIGameplay.Get().FadeOutAndIn(moveIn);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        playerMovement.CopyPositionAndRotation(moveIn ? insideOfLabPos : outsideOfLabPos);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingIn);
        ChangeCurrentZone(moveIn ? labZone : initialZone);
        ChangeInputState(InputState.Movement);
    }

    void ChangeCurrentZone(ZoneData newZone)
    {
        currentZone = newZone;
        playerMovement.LoadZoneData(currentZone.allowHook, currentZone.allowBeacons);
    }

    public void DiscoverHook()
    {
        LevelsManager.Get().persistentData.hookDiscovered = true;
        LoadPlayerPersistence();
    }

    public void DiscoverBeacons()
    {
        LevelsManager.Get().persistentData.beaconsDiscovered = true;
        LoadPlayerPersistence();
    }

    public void DiscoverSonar()
    {
        LevelsManager.Get().persistentData.sonarDiscovered = true;
        LoadPlayerPersistence();
    }

    public bool IsZoneDone()
    {
        return !GetCurrentZone().HasNecessaryInteractionLeft();
    } 
}
