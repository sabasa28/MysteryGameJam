using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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
    [SerializeField] ZoneData lowerCavesReturnZone;
    [SerializeField] Transform outsideOfShipPos;
    [SerializeField] Transform insideOfShipPos;
    [SerializeField] Transform outsideOfLabPos;
    [SerializeField] Transform insideOfLabPos;
    [SerializeField] Volume helmetVolume;
    bool inGameMenuOpen = false;
    bool optionsMenuOpen = false;
    [SerializeField] SelectingFinger selectingFinger;
    [SerializeField] bool isPlayerInShip = true;
    [SerializeField] bool isPlayerInLab = false;
    InputState savedInputState;
    [SerializeField] GameObject playerDocs;
    bool playerDocsActive;

    private void Start()
    {
        LevelsManager levelsManager = LevelsManager.Get();
        if (!levelsManager.GoingUp && levelsManager.GetCurrentSceneName() == "SurfaceScene")
        {
            ChangeInputState(InputState.Cinematic);
            playerMovement.InitialPlayerSpawn();
        }
        else
        { 
            ChangeInputState(InputState.Movement);
        }
        playerDocsActive = playerDocs.activeInHierarchy;
        SetPlayerDocsActiveState(playerDocsActive);
        initialZone = currentZone;
        ChangeCurrentZone(currentZone); // not really changing just loading playerdata
    }
    private void Update()
    {

        if ((Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Tab)) && playerMovement.CanModifyTabletState() && inputState != InputState.Chat && inputState != InputState.Cinematic)
        {
            inGameMenuOpen = !inGameMenuOpen;
            ChangeInputState(inGameMenuOpen ? InputState.InGameUI : InputState.Movement);

            if (inGameMenuOpen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                playerMovement.RaiseHand();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
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
        playerMovement.StopEnablingInputCoroutine(); //in case we try to change input right after changing input to movement, which takes one frame
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
        playerMovement.SetJumpAllowed(!isPlayerInShip);
    }

    public bool IsInShip()
    {
        return isPlayerInShip;
    }
    
    public bool IsInLab()
    {
        return isPlayerInLab;
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

    public void MovePlayerInOutOfShip(float customFadeOutInTime = -1.0f)
    {
        if (customFadeOutInTime > 0.0f)
        {
            UIGameplay.Get().SetCustomTimeForNextFade(customFadeOutInTime);
        }
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
        UIGameplay.Get().FadeOutAndIn(false);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        playerMovement.GetComponent<CharacterController>().enabled = false;
        playerMovement.transform.position = moveIn ? insideOfShipPos.position : outsideOfShipPos.position;
        playerMovement.GetComponent<CharacterController>().enabled = true;
        helmetVolume.enabled = !moveIn;
        LevelsManager.Get().persistentData.UpdateHelmetState(!moveIn);
        playerMovement.PlayHelmetSound(!moveIn);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingIn);
        ChangeInputState(InputState.Movement);
        playerMovement.SetJumpAllowed(!moveIn);
    }

    IEnumerator MovePlayerInOutLabCoroutine(bool moveIn)
    {
        ChangeInputState(InputState.Cinematic);
        UIGameplay.Get().FadeOutAndIn(moveIn);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        if (!moveIn)
        {
            LevelsManager.Get().persistentData.isReturning = true;
        }
        playerMovement.CopyPositionAndRotation(moveIn ? insideOfLabPos : outsideOfLabPos);
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingIn);
        ChangeCurrentZone(moveIn ? labZone : lowerCavesReturnZone);
        ChangeInputState(InputState.Movement);
    }

    public void OnInitialAnimationEnded()
    {
        ChatManager.Get().PlayWakeUpChat();
        StartCoroutine(WaitUntilChatEndAndOpenTablet());
    }

    IEnumerator WaitUntilChatEndAndOpenTablet()
    {
        yield return new WaitUntil(() => inputState == InputState.Movement);
    }

    void ChangeCurrentZone(ZoneData newZone)
    {
        currentZone = newZone;
        newZone.CheckNecessaryInteractions();
        ChatManager.Get().doneWithZone = false;
        playerMovement.LoadZoneData(currentZone.allowHook, currentZone.allowBeacons, currentZone.allowFlashlight);
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

    public void DiscoverCalendar()
    {
        LevelsManager.Get().persistentData.calendarDiscovered = true;
    }

    public bool IsZoneDone()
    {
        return !GetCurrentZone().HasNecessaryInteractionLeft();
    }

    public void StartEndGameCinematic()
    {
        ChangeInputState(InputState.Cinematic);
        MovePlayerInOutOfShip(3.0f);
        StartCoroutine(StartEndAnimation());
    }

    IEnumerator StartEndAnimation()
    { 
        yield return new WaitUntil(() => !UIGameplay.Get().isFadingOut);
        yield return new WaitUntil(() => !UIGameplay.Get().isFaded);
        Animator playerAnim = playerMovement.GetComponent<Animator>();
        playerAnim.enabled = true;
        playerAnim.SetBool("HasReceivedTerribleNews", true);
    }

    public void SwitchToResultsScene()
    {
        SceneManager.LoadScene("ResultsScreenScene");
    }

    public bool TabletEnabled()
    {
        return playerMovement.IsTabletEnabled();
    }

    public void EnableAndOpenTablet()
    {
        playerMovement.EnableTablet();
        inGameMenuOpen = true;
        ChangeInputState(InputState.InGameUI);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        playerMovement.RaiseHand();
    }

    public void MovePlayerCameraAndReturn(Transform targetPos, float goingTime, float returningTime)
    {
        ChangeInputState(InputState.InGameUI);
        playerMovement.MoveCameraAndReturn(targetPos, goingTime, returningTime);
    }

    public void ReturnPlayerCamera()
    { 
        playerMovement.ReturnCameraToPlayer();
    }
}
