using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ended up using it not only for movement
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float cameraSpeed;
    [SerializeField] float gravityForce;
    [SerializeField] float jumpForce;
    [SerializeField] float verticalForce;
    [SerializeField] bool testgrounded;
    [SerializeField] float coyoteTime;
    bool grounded = true;
    float lastTimeGrounded;
    [SerializeField] bool isFlashlightEnabled;
    [SerializeField] GameObject flashlight;
    [SerializeField] AudioClip flashlightSound;
    [SerializeField] AudioClip sonarSound;
    [SerializeField] AudioClip footstepsDirtSound;
    [SerializeField] AudioClip footstepsMetalSound;
    [SerializeField] AudioClip helmetOnSound;
    [SerializeField] AudioClip helmetOffSound;
    [SerializeField] AudioClip inventoryOnOffSound;
    [SerializeField] AudioClip hookSound;
    float cameraRotX = 0.0f;
    float cameraRotY;
    CharacterController characterController;
    [SerializeField] Camera mainCamera;
    Vector3 movement;
    RaycastHit interactHit;
    RaycastHit hookHit;
    [SerializeField] float maxInteractDistance;
    [SerializeField] float maxHookDistance;
    [SerializeField] float hookPullSpeed;
    [SerializeField] float hookCooldown;
    [SerializeField] float maxTimeHooked;
    [SerializeField] float timeBeforeGoingToHook;
    bool goingToHook = false;
    float hookMinTimeToReUse = 0.0f;
    bool inputEnabled = true;
    [SerializeField] float sonarTimer;
    [SerializeField] float sonarAlignTime;
    bool isSonarActive = false;
    [SerializeField] SpriteRenderer sonarSprite;
    [SerializeField] SpriteRenderer sonarSprite2;
    [SerializeField] Transform sonarPivotY;
    [SerializeField] Transform sonarPivotX;
    [SerializeField] GameObject sonar;
    Transform trackedTransform;
    [SerializeField] Color closeColor;
    [SerializeField] Color mediumColor;
    [SerializeField] Color farColor;
    [SerializeField] float distanceForFarColor;
    [SerializeField] float distanceForMediumColor;
    [SerializeField] Hook hook;
    IInteractable interactable;
    [SerializeField] float distToFloor; //es un toque mas en verdad porque flota un toque
    [SerializeField] GameObject beaconPrefab;
    [SerializeField] int maxBeacons;
    [SerializeField] LayerMask defaultLayer;
    int beaconsPlaced = 0;
    [SerializeField] PersistentData persistentData; //ignore warcrime
    [SerializeField] GameObject uiHand;
    [SerializeField] float uiHandXrotationDown;
    [SerializeField] float uiHandXrotationUp;
    [SerializeField] float uiHandMovingTime;
    [SerializeField] AnimationCurve uiHandCurve;
    [SerializeField] Animator uiHandAnim;
    bool isAnimatingUiHand = false;
    bool walking = false;
    [SerializeField] float minMagnitudeForSteps;
    [SerializeField] float pitchForDirtSteps;
    [SerializeField] float pitchForMetalSteps;
    [SerializeField] float volumeDirtSteps;
    [SerializeField] float volumeMetalSteps;
    bool playingStepsSound = false;
    AudioClip overridenStepsSound = null;
    bool hookDiscovered;
    bool beaconsDiscovered;
    bool sonarDiscovered;
    bool hookAllowed;
    bool beaconsAllowed;
    bool flashlightAllowed;
    bool jumpAllowed = false;
    bool tabletAllowed = true;
    Animator animator;
    Coroutine enablingInputCoroutine = null;
    [SerializeField] AudioClip endAnimMetal1;
    [SerializeField] AudioClip endAnimMetal2;
    [SerializeField] AudioClip endAnimMetal3;
    [SerializeField] AudioClip endAnimMetal4;
    [SerializeField] AudioClip endAnimDirt1;
    int endAnimSteps = 0;
    bool triggerCameraReturn = false;
    bool cameraDettached = false;

    void Awake()
    {
        Time.timeScale = 1.0f;//quitar
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!LevelsManager.Get().playedInitialAnimation)
        {
            animator.enabled = true;
            LevelsManager.Get().playedInitialAnimation = true;
        }
        beaconsPlaced = persistentData.GetBeaconsUsed();
        Transform initialTransform = LevelsManager.Get().GoingUp? GameplayController.Get().GetCurrentZone().exit : GameplayController.Get().GetCurrentZone().entrance;
        CopyPositionAndRotation(initialTransform);
    }

    void Update()
    {
        float horizontalValue = 0.0f;
        float verticalValue = 0.0f;
        float mouseY = 0.0f;
        float mouseX = 0.0f;
        if (inputEnabled && !GameplayController.Get().IsCameraLocked())
        {
            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            horizontalValue = Input.GetAxis("Horizontal");
            verticalValue = Input.GetAxis("Vertical");
            if (Input.GetButtonDown("Jump") && jumpAllowed && grounded && verticalForce < 0.0f)
            {
                verticalForce += jumpForce;
            }
            if (Input.GetKeyDown(KeyCode.F) && flashlightAllowed)
            {
                SetFlashlightState(!isFlashlightEnabled);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ActivateSonar();
            }
            if (Input.GetKeyDown(KeyCode.E) && interactable != null)
            {
                interactable.AttemptInteract();
            }
            if (Input.GetKeyDown(KeyCode.Q) && hookAllowed && hookDiscovered && Time.time > hookMinTimeToReUse && !hook.gameObject.activeInHierarchy)
            {
                HookRaycast();
            }
            if (Input.GetKeyDown(KeyCode.R) && beaconsAllowed && beaconsDiscovered)
            {
                PlaceBeacon();
            }
        }

        float magnitudeMovement = Mathf.Max(Mathf.Abs(horizontalValue), Mathf.Abs(verticalValue));
        movement = new Vector3(verticalValue, horizontalValue).normalized * magnitudeMovement;

        Vector3 forwardNoY = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z).normalized;
        Vector3 rightNoY = new Vector3(mainCamera.transform.right.x, 0.0f, mainCamera.transform.right.z).normalized;
        movement = forwardNoY * movement.x + rightNoY * movement.y;

        if (walking && (movement.magnitude < minMagnitudeForSteps || !grounded))
        {
            walking = false;
            AudioManager.Get().StopSteps();
            playingStepsSound = false;
        }
        if (!walking && (movement.magnitude > minMagnitudeForSteps && grounded))
        {
            walking = true;
            PlayStepSound();
        }

        testgrounded = characterController.isGrounded;
        if (!cameraDettached)
        {
            cameraRotX += cameraSpeed * SettingsData.sensitivity * 2.0f * -mouseY;
            cameraRotX = Mathf.Clamp(cameraRotX, -89.0f, 89.0f);
            cameraRotY += cameraSpeed * SettingsData.sensitivity * 2.0f * mouseX;
            mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraRotX);
            transform.rotation = Quaternion.Euler(Vector3.up * cameraRotY);
        }
        RayoLaser();
    }

    private void FixedUpdate()
    {
        if (grounded != characterController.isGrounded)
        {
            if (grounded)
            {
                StartCoroutine(CoyoteTime());
            }
            else
            {
                StopCoroutine(CoyoteTime());
                grounded = characterController.isGrounded;
            }
        }


        verticalForce = (characterController.isGrounded && verticalForce < 0.0f) || goingToHook ? -1.0f : verticalForce + gravityForce;

        characterController.Move((speed * movement + (Vector3.up * verticalForce)) * Time.fixedDeltaTime);
    }

    private void RayoLaser()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out interactHit, maxInteractDistance))
        {
            interactable = interactHit.collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = interactHit.collider.GetComponentInParent<IInteractable>();
            }
            if (interactable != null && inputEnabled)
            {
                UIGameplay.Get().ChangeInteractTextDisplay(interactable.IsInteractable());
            }
            else
            {
                UIGameplay.Get().ChangeInteractTextDisplay(false);
            }
        }
        else
        {
            interactable = null;
            UIGameplay.Get().ChangeInteractTextDisplay(false);
        }
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxInteractDistance, Color.green, 0.1f);
    }

    private void HookRaycast()
    {
        AudioManager.Get().PlaySFX(hookSound, 0.2f);
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hookHit, maxHookDistance, defaultLayer, QueryTriggerInteraction.Ignore))
        {
            hook.gameObject.SetActive(true);
            hook.SetTargetPos(hookHit.point);
            StartCoroutine(GoToHookHit());
            hookMinTimeToReUse = Time.time + hookCooldown;
        }
        else
        {
            hook.gameObject.SetActive(true);
            hook.SetTargetPos(mainCamera.transform.position + mainCamera.transform.forward * maxHookDistance, false);
        }
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxHookDistance, Color.green, 20.0f);
    }

    public IEnumerator GoToHookHit()
    {
        yield return new WaitUntil(() => hook.hookReachedTarget);
        yield return new WaitForSeconds(timeBeforeGoingToHook);
        goingToHook = true;
        float maxHitTimer = Time.time + maxTimeHooked;
        while ((Vector3.Distance(transform.position, hookHit.point) > 2.0f) && Time.time <= maxHitTimer)
        {
            characterController.Move(hookPullSpeed * Time.fixedDeltaTime * (hookHit.point - transform.position));
            yield return new WaitForFixedUpdate();
        }
        goingToHook = false;
        hook.gameObject.SetActive(false);
    }


    public void SetInputState(bool newState)
    {
        if (newState)
        {
            enablingInputCoroutine = StartCoroutine(WaitAndEnableInput()); //we do this hack because otherwise when we re-enabled the input using an input that is used for movement it will also trigger. example: skiping the last dialog will make you jump
        }
        else
        {
            inputEnabled = newState;
        }
    }

    public void StopEnablingInputCoroutine()
    {
        if (enablingInputCoroutine != null)
        {
            StopCoroutine(enablingInputCoroutine);
        }
    }
    IEnumerator WaitAndEnableInput()
    {
        yield return new WaitForEndOfFrame();
        inputEnabled = true;
    }

    public void SetFlashlightState(bool enabled, bool playSound = true)
    {
        if (enabled == isFlashlightEnabled)
        {
            return;
        }
        isFlashlightEnabled = enabled;
        flashlight.SetActive(enabled);
        if (playSound) AudioManager.Get().PlaySFX(flashlightSound, 1);
        persistentData.UpdateFlashlightState(enabled);
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        grounded = characterController.isGrounded;
    }

    public void ActivateSonar()
    {
        if (isSonarActive || !sonarDiscovered)
        {
            return;
        }
        if (GameplayController.Get().GetCurrentZone().GetClosestInteractable(transform.position, out GameObject closestInteractable))
        {
            StartCoroutine(DisplaySonarArrow(closestInteractable.transform));
        }
        else
        {
            ChatManager.Get().PlayDoneWithZoneChat();
            if (GameplayController.Get().GetCurrentZone().exitDoor)
            {
                StartCoroutine(DisplaySonarArrow(GameplayController.Get().GetCurrentZone().exitDoor));
            }
        }
    }

    IEnumerator DisplaySonarArrow(Transform closestInteractableTransform)
    {
        trackedTransform = closestInteractableTransform;
        Vector3 interactableTrackedPos = trackedTransform.position;
        AudioManager.Get().PlaySFX(sonarSound, 1.0f);
        sonar.SetActive(true);
        isSonarActive = true;
        float timer = 0.0f;
        while (timer < sonarTimer && trackedTransform != null)
        {
            Vector3 slerpedVector = Vector3.Slerp(transform.forward, interactableTrackedPos - transform.position, timer / sonarAlignTime);
            Quaternion rotation = Quaternion.LookRotation(slerpedVector, Vector3.up);
            float distToInteractable = Vector3.Distance(transform.position, interactableTrackedPos);
            if (distToInteractable > distanceForMediumColor)
            {
                sonarSprite.color = Color.Lerp(mediumColor, farColor, (distToInteractable - distanceForMediumColor) / (distanceForFarColor - distanceForMediumColor));
                sonarSprite2.color = Color.Lerp(mediumColor, farColor, (distToInteractable - distanceForMediumColor) / (distanceForFarColor - distanceForMediumColor));
            }
            else
            {
                sonarSprite.color = Color.Lerp(closeColor, mediumColor, distToInteractable / distanceForMediumColor);
                sonarSprite2.color = Color.Lerp(closeColor, mediumColor, distToInteractable / distanceForMediumColor);
            }
            sonarPivotY.eulerAngles = new Vector3(sonarPivotY.eulerAngles.x, rotation.eulerAngles.y, sonarPivotY.eulerAngles.z);
            sonarPivotX.eulerAngles = new Vector3(rotation.eulerAngles.x, sonarPivotX.eulerAngles.y, sonarPivotX.eulerAngles.z);
            timer += Time.deltaTime;
            yield return null;
        }
        isSonarActive = false;
        sonar.SetActive(false);
        trackedTransform = null;
    }

    public void StopTrackingFoundInteractable(GameObject interactableFound)
    {
        if (trackedTransform != null && trackedTransform == interactableFound.transform)
        {
            trackedTransform = null;
            //this will stop the sonar coroutine
        }
    }

    void PlaceBeacon()
    {
        if (beaconsPlaced >= maxBeacons)
        {
            return;
        }
        //if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit beaconHit, (distToFloor + 0.2f/*small offset just in case*/) * transform.localScale.y, defaultLayer))
        //{
        //    Instantiate(beaconPrefab, beaconHit.point, Quaternion.identity);
        //    beaconsPlaced++;
        //    persistentData.AddBeacon(beaconHit.point, LevelsManager.Get().GetCurrentSceneName());
        //}
        Vector3 spawnPos = transform.position + Vector3.down;
        Instantiate(beaconPrefab, spawnPos, Quaternion.identity);
        beaconsPlaced++;
        persistentData.AddBeacon(spawnPos, LevelsManager.Get().GetCurrentSceneName());
    }

    public void SpawnPersistentBeacons()
    {
        List<Vector3> beaconsToSpawn = LevelsManager.Get().beaconsPosInLoadedLevel;
        if (beaconsToSpawn != null)
        {
            foreach (Vector3 posToSpawnBeacon in beaconsToSpawn)
            {
                Instantiate(beaconPrefab, posToSpawnBeacon, Quaternion.identity);
            }
        }
    }

    public void LoadPersistentData()
    {
        beaconsDiscovered = persistentData.beaconsDiscovered;
        hookDiscovered = persistentData.hookDiscovered;
        sonarDiscovered = persistentData.sonarDiscovered;
        SetFlashlightState((persistentData.flashlightOn && flashlightAllowed));
    }

    public void LoadZoneData(bool inHookAllowed, bool inBeaconsAllowed, bool inFlashLightAllowed)
    {
        hookAllowed = inHookAllowed;
        beaconsAllowed = inBeaconsAllowed;
        flashlightAllowed = inFlashLightAllowed;
        if (!flashlightAllowed && isFlashlightEnabled)
        {
            SetFlashlightState(false, false);
        }
    }

    public void RaiseHand()
    {
        StartCoroutine(MoveHand(true));
    }
    public void LowerHand()
    {
        StartCoroutine(MoveHand(false));
    }

    IEnumerator MoveHand(bool raise)
    {
        isAnimatingUiHand = true;
        if (raise)
        {
            uiHand.SetActive(true);
        }
        else
        {
            AudioManager.Get().PlaySFX(inventoryOnOffSound);
            uiHandAnim.SetBool("Open", false);
            GameplayController.Get().SetPlayerDocsActiveState(false);
            GameplayController.Get().SetFingerActiveState(false);
            yield return new WaitUntil(() => uiHandAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed"));
        }
        Quaternion initialRot = Quaternion.Euler(new Vector3(raise ? uiHandXrotationDown : uiHandXrotationUp, uiHand.transform.localRotation.y, uiHand.transform.localRotation.z));
        Quaternion endRot = Quaternion.Euler(new Vector3(raise ? uiHandXrotationUp : uiHandXrotationDown, uiHand.transform.localRotation.y, uiHand.transform.localRotation.z));
        float timer = 0.0f;
        float t;
        while (timer <= uiHandMovingTime)
        {
            timer += Time.deltaTime;
            t = uiHandCurve.Evaluate(timer / uiHandMovingTime);
            uiHand.transform.localRotation = Quaternion.Lerp(initialRot, endRot, t);
            yield return null;
        }
        if (raise)
        {
            AudioManager.Get().PlaySFX(inventoryOnOffSound);
            uiHandAnim.SetBool("Open", true);
            yield return new WaitUntil(() => uiHandAnim.GetCurrentAnimatorStateInfo(0).IsName("Opened"));
            GameplayController.Get().SetPlayerDocsActiveState(true);
            GameplayController.Get().SetFingerActiveState(true);
        }
        else
        {
            uiHand.SetActive(false);
        }
        isAnimatingUiHand = false;
    }

    public bool CanModifyTabletState()
    {
        return !isAnimatingUiHand && tabletAllowed;
    }

    public bool IsTabletEnabled()
    {
        return tabletAllowed;
    }

    public void EnableTablet()
    {
        tabletAllowed = true;

    }

    public void CopyPositionAndRotation(Transform transformToCopy)
    {
        characterController.enabled = false;
        transform.position = transformToCopy.position;
        transform.rotation = transformToCopy.rotation;
        cameraRotX = transformToCopy.rotation.eulerAngles.x;
        cameraRotY = transformToCopy.rotation.eulerAngles.y;
        characterController.enabled = true;
    }

    public void InitialPlayerSpawn()
    {
        tabletAllowed = false;
        UIGameplay.Get().FadeIn();
        animator.SetTrigger("InitialAnimation");
    }

    public void OnInitialAnimationEnded()
    {
        GameplayController.Get().OnInitialAnimationEnded();
        animator.enabled = false;
    }

    public void PlayHelmetSound(bool helmetOn)
    {
        if (helmetOn)
        {
            AudioManager.Get().PlaySFX(helmetOnSound);
        }
        else
        { 
            AudioManager.Get().PlaySFX(helmetOffSound);
        }
    }

    public void ForceDirtSteps(bool shouldForce)
    {
        overridenStepsSound = shouldForce? footstepsDirtSound : null;
        if (playingStepsSound)
        {
            AudioManager.Get().StopSteps();
            PlayStepSound();
        }
    }

    public void PlayStepSound()
    {
        if (overridenStepsSound != null)
        {
            AudioManager.Get().PlaySteps(overridenStepsSound, pitchForDirtSteps, volumeDirtSteps); //I'm only overriding for dirt, if we end up overriding for other mats I'll have to deal with pitch and volume diffs
        }
        else if (GameplayController.Get().IsInShip() || GameplayController.Get().IsInLab())
        {
            AudioManager.Get().PlaySteps(footstepsMetalSound, pitchForMetalSteps, volumeMetalSteps);
            playingStepsSound = true;
        }
        else
        {
            AudioManager.Get().PlaySteps(footstepsDirtSound, pitchForDirtSteps, volumeDirtSteps);
            playingStepsSound = true;
        }
    }

    public void EndGame()
    {
        GameplayController.Get().SwitchToResultsScene();
    }

    //solo mi padre puede juzgarme
    public void PlayAnimStep()
    {
        endAnimSteps++;
        switch (endAnimSteps)
        {
            case 1:
                AudioManager.Get().PlayIndividualStep(endAnimMetal1, pitchForMetalSteps, volumeMetalSteps);
                break;
            case 2:
                AudioManager.Get().PlayIndividualStep(endAnimMetal2, pitchForMetalSteps, volumeMetalSteps);
                break;
            case 3:
                AudioManager.Get().PlayIndividualStep(endAnimMetal3, pitchForMetalSteps, volumeMetalSteps);
                break;
            case 4:
                AudioManager.Get().PlayIndividualStep(endAnimMetal4, pitchForMetalSteps, volumeMetalSteps);
                break;
            case 5:
            case 6:
            default:
                AudioManager.Get().PlayIndividualStep(endAnimDirt1, pitchForDirtSteps, volumeDirtSteps);
                break;
        }
    }

    public void SetJumpAllowed(bool newJumpAllowed)
    { 
        jumpAllowed = newJumpAllowed;
    }

    public void MoveCameraAndReturn(Transform targetTransform, float goingTime, float returningTime)
    {
        StartCoroutine(MoveCameraAndReturnCoroutine(targetTransform, goingTime, returningTime));
    }

    IEnumerator MoveCameraAndReturnCoroutine(Transform newPosNRot, float goingTime, float returningTime)
    {
        float timer = 0.0f;
        Vector3 initialCamPos = mainCamera.transform.position;
        Quaternion initialCamRot = mainCamera.transform.rotation;
        Vector3 initialLocalCamPos = mainCamera.transform.localPosition;
        Quaternion initialLocalCamRot = mainCamera.transform.localRotation;
        cameraDettached = true;
        while (timer < goingTime)
        {
            mainCamera.transform.SetPositionAndRotation(Vector3.Lerp(initialCamPos, newPosNRot.position, timer/goingTime), Quaternion.Slerp(initialCamRot, newPosNRot.rotation, timer/goingTime));
            timer += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.SetPositionAndRotation(newPosNRot.position, newPosNRot.rotation);
        timer = 0.0f;

        yield return new WaitUntil(() => triggerCameraReturn == true);
        Vector3 posToLocal = mainCamera.transform.localPosition;
        Quaternion rotToLocal = mainCamera.transform.localRotation;
        while (timer < returningTime)
        {
            mainCamera.transform.SetLocalPositionAndRotation(Vector3.Lerp(posToLocal, initialLocalCamPos, timer / returningTime), Quaternion.Slerp(rotToLocal, initialLocalCamRot, timer / returningTime));
            
            timer += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.SetLocalPositionAndRotation(initialLocalCamPos, initialLocalCamRot);
        triggerCameraReturn = false;
        cameraDettached = false;
    }

    public void ReturnCameraToPlayer()
    {
        triggerCameraReturn = true;
    }
}
