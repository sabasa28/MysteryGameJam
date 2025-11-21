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
    [SerializeField] Volume noGlassessVolume;
    [SerializeField] StormManager stormManager;
    bool inGameMenuOpen = false;
    bool optionsMenuOpen = false;
    [SerializeField] SelectingFinger selectingFinger;
    [SerializeField] bool isPlayerInShip = true;
    [SerializeField] bool isPlayerInLab = false;
    InputState savedInputState;
    [SerializeField] GameObject playerDocs;
    bool playerDocsActive;
    [SerializeField] Transform currentUnstuckCheckpoint;
    bool unstuckCheckpointSet = false;
    bool savedCursorVisibility = false;
    UIGameplay uiGameplay;

    private void Start()
    {
        LevelsManager levelsManager = LevelsManager.Get();
        uiGameplay = UIGameplay.Get();
        if (!levelsManager.GoingUp && levelsManager.GetCurrentSceneName() == "SurfaceScene")
        {
            ChangeInputState(InputState.Cinematic);
            playerMovement.InitialPlayerSpawn();
            AudioManager.Get().UpdateBackgroundVolume(0.5f, false);
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
        if (Input.GetKeyDown(KeyCode.Escape) && ((inputState == InputState.Movement && !optionsMenuOpen) || (inputState == InputState.UI && optionsMenuOpen)) &&
            !playerMovement.IsAnimating() && !playerMovement.CameraIsDettached() && !uiGameplay.IsAnyDocActive() && !IsOnAnyTransition())
        {
            OnUIMenuStateChanged();
            uiGameplay.ChangeMenuVisibility(optionsMenuOpen);
        }
        //es muy tonto que esto este aca
        if ((Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Tab) || (inGameMenuOpen && Input.GetKeyDown(KeyCode.Escape))) && playerMovement.CanModifyTabletState() && inputState != InputState.Chat && inputState != InputState.Cinematic 
            && !IsOptionsMenuOpen() && !playerMovement.IsAnimating() && !playerMovement.CameraIsDettached() && !uiGameplay.IsAnyDocActive() && !IsOnAnyTransition()) //https://youtu.be/0LUYV3a1qgA?t=8
        {
            inGameMenuOpen = !inGameMenuOpen;
            ChangeInputState(inGameMenuOpen ? InputState.InGameUI : InputState.Movement);

            if (inGameMenuOpen)
            {
                OpenTablet();
            }
            else
            {
                CloseTablet();
            }
        }

    }

    void OpenTablet()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        playerMovement.RaiseHand();
    }

    void CloseTablet()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement.LowerHand();
    }

    public void OnUIMenuStateChanged()
    {
        optionsMenuOpen = !optionsMenuOpen;
        if (optionsMenuOpen)
        {
            savedInputState = inputState;
            if (savedInputState == InputState.UI)
            {
                savedCursorVisibility = Cursor.visible;
            }
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
                case InputState.UI:
                    Cursor.visible = savedCursorVisibility;
                    Cursor.lockState = CursorLockMode.Confined;
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
        if (newState != InputState.UI)
        {
            savedInputState = newState;
        }
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
    public void SetNoGlassesVolumeState(bool newState)
    {
        if (noGlassessVolume)
        {
            noGlassessVolume.enabled = newState;
            playerMovement.PutOnGlasses();
        }
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
            uiGameplay.SetCustomTimeForNextFade(customFadeOutInTime);
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
        playerMovement.StopTrackingCurrentTracked();
        ChangeInputState(InputState.Cinematic);
        uiGameplay.FadeOutAndIn(false);
        yield return new WaitUntil(() => !uiGameplay.isFadingOut);
        playerMovement.CopyPositionAndRotation(moveIn ? insideOfShipPos : outsideOfShipPos);
        helmetVolume.enabled = !moveIn;
        LevelsManager.Get().persistentData.UpdateHelmetState(!moveIn);
        playerMovement.PlayHelmetSound(!moveIn);
        stormManager.OnEnterOrExitShip(moveIn);
        yield return new WaitUntil(() => !uiGameplay.isFadingIn);
        ChangeInputState(InputState.Movement);
        playerMovement.SetJumpAllowed(!moveIn);
        AudioManager.Get().UpdateBackgroundVolume(moveIn ? 0.5f : 1.0f, true);
    }

    IEnumerator MovePlayerInOutLabCoroutine(bool moveIn)
    {
        playerMovement.StopTrackingCurrentTracked();
        ChangeInputState(InputState.Cinematic);
        uiGameplay.FadeOutAndIn(moveIn);
        yield return new WaitUntil(() => !uiGameplay.isFadingOut);
        if (!moveIn)
        {
            LevelsManager.Get().persistentData.isReturning = true;
        }
        playerMovement.CopyPositionAndRotation(moveIn ? insideOfLabPos : outsideOfLabPos);
        yield return new WaitUntil(() => !uiGameplay.isFadingIn);
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
        yield return new WaitUntil(() => !uiGameplay.isFadingOut);
        yield return new WaitUntil(() => !uiGameplay.isFaded);
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

    public void FlickerAndDisablePlayerFlashlight()
    {
        playerMovement.FlickerAndDisableFlashlight();
    }

    public void ReenablePlayerFlashlight(float timeBeforeReenabling)
    {
        StartCoroutine(ReenablePlayerFlashlightAfterSecs(timeBeforeReenabling));
    }

    IEnumerator ReenablePlayerFlashlightAfterSecs(float secs)
    {
        yield return new WaitForSeconds(secs);
        playerMovement.ReenableDisabledFlashlight();
    }

    public void MovePlayerCameraAndReturn(Transform targetPos, float goingTime, float returningTime, InputState newInputState = InputState.InGameUI)
    {
        ChangeInputState(newInputState);
        playerMovement.MoveCameraAndReturn(targetPos, goingTime, returningTime);
    }

    public void ReturnPlayerCamera(bool enableMovement)
    { 
        playerMovement.ReturnCameraToPlayer(enableMovement);
    }

    public void OnCurrentZoneNecessaryInteractableFound(GameObject InteractableFound)
    {
        playerMovement.StopTrackingFoundInteractable(InteractableFound);
    }

    public void ForcePlayerSonar()
    {
        playerMovement.ActivateSonar();
    }

    public void ForcePlayerFlashlight(bool newState)
    {
        playerMovement.SetFlashlightState(newState);
    }

    public bool IsOptionsMenuOpen()
    {
        return optionsMenuOpen;
    }

    public void UnstuckPlayer()
    {
        if (unstuckCheckpointSet && !chatManager.IsInTextMode() && !playerMovement.IsAnimating() && !playerMovement.CameraIsDettached() && !inGameMenuOpen && !IsOnAnyTransition() && !uiGameplay.IsAnyDocActive() && !playerMovement.IsHooking())
        {
            playerMovement.CopyPositionAndRotation(currentUnstuckCheckpoint);
        }
    }

    public void SetUnstuckCheckpoint(Transform newCheckpointTrans)
    {
        currentUnstuckCheckpoint = newCheckpointTrans;
        unstuckCheckpointSet = true; 
    }

    public bool IsInGameMenuOpen()
    {
        return inGameMenuOpen;
    }

    public bool IsOnAnyTransition()
    {
        return uiGameplay.IsOnAnyFadeState();
    }

    public void SetFlashlightSettings(float range, float innerIntensity, float outerIntensity)
    {
        playerMovement.SetFlashlightSettings(range, innerIntensity, outerIntensity);
    }
    public void GetFlashlightSettings(out float range, out float innerIntensity, out float outerIntensity)
    {
        playerMovement.GetFlashlightSettings(out range, out innerIntensity, out outerIntensity); //ref
    }
    public void StartBreakingFlashlight()
    {
        playerMovement.FlashlightStartsFailing();
    }
}
