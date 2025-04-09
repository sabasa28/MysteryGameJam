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
    float cameraRotX = 0.0f;
    float cameraRotY;
    CharacterController characterController;
    Camera mainCamera;
    Vector3 movement;
    RaycastHit interactHit;
    [SerializeField] float maxInteractDistance;
    bool inputEnabled = true;
    [SerializeField] float sonarTimer;
    [SerializeField] float sonarAlignTime;
    bool isSonarActive = false;
    [SerializeField] SpriteRenderer sonarSprite;
    [SerializeField] Transform sonarPivotY;
    [SerializeField] Transform sonarPivotX;
    [SerializeField] GameObject sonar;
    [SerializeField] Color closeColor;
    [SerializeField] Color mediumColor;
    [SerializeField] Color farColor;
    [SerializeField] float distanceForFarColor;
    [SerializeField] float distanceForMediumColor;
    IInteractable interactable;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetFlashlightState(isFlashlightEnabled);
    }

    void Update()
    {
        float horizontalValue = 0.0f;
        float verticalValue = 0.0f;
        float mouseY = 0.0f;
        float mouseX = 0.0f;
        if (inputEnabled)
        {
            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            horizontalValue = Input.GetAxis("Horizontal");
            verticalValue = Input.GetAxis("Vertical");
            if (Input.GetButtonDown("Jump") && grounded && verticalForce < 0.0f)
            {
                verticalForce += jumpForce;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                SetFlashlightState(!isFlashlightEnabled);
            }
            if (Input.GetKeyDown(KeyCode.G) && !isSonarActive)
            {
                ActivateSonar();
            }
            if (Input.GetKeyDown(KeyCode.E) && interactable != null)
            {
                interactable.AttemptInteract();
            }
        }

        float magnitudeMovement = Mathf.Max(Mathf.Abs(horizontalValue), Mathf.Abs(verticalValue));
        movement = new Vector3(verticalValue, horizontalValue).normalized * magnitudeMovement;

        Vector3 forwardNoY = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z).normalized;
        Vector3 rightNoY = new Vector3(mainCamera.transform.right.x, 0.0f, mainCamera.transform.right.z).normalized;
        movement = forwardNoY * movement.x + rightNoY * movement.y;

        testgrounded = characterController.isGrounded;

        cameraRotX += cameraSpeed * -mouseY;
        cameraRotX = Mathf.Clamp(cameraRotX, -89.0f, 89.0f);
        cameraRotY += cameraSpeed * mouseX;
        mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraRotX);
        transform.rotation = Quaternion.Euler(Vector3.up * cameraRotY);
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


        verticalForce = characterController.isGrounded && verticalForce < 0.0f ? -1.0f : verticalForce + gravityForce;
        
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
            if (interactable != null)
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
            UIGameplay.Get().ChangeInteractTextDisplay(false);
        }
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxInteractDistance, Color.green, 0.1f);
    }

    public void SetInputState(bool newState)
    {
        if (newState)
        {
            StartCoroutine(WaitAndEnableInput()); //we do this hack because otherwise when we re-enabled the input using an input that is used for movement it will also trigger. example: skiping the last dialog will make you jump
        }
        else
        {
            inputEnabled = newState;
        }
    }
    IEnumerator WaitAndEnableInput()
    {
        yield return new WaitForEndOfFrame();
        inputEnabled = true;
    }

    void SetFlashlightState(bool enabled)
    {
        isFlashlightEnabled = enabled;
        flashlight.SetActive(enabled);
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        grounded = characterController.isGrounded;
    }

    void ActivateSonar()
    {
        Vector3 closestInteractable;
        GameplayController.Get().GetCurrentZone().GetClosestInteractable(transform.position, out closestInteractable);
        StartCoroutine(DisplaySonarArrow(closestInteractable));
    }

    IEnumerator DisplaySonarArrow(Vector3 closestInteractablePos)
    {
        sonar.SetActive(true);
        isSonarActive = true;
        float timer = 0.0f;
        while (timer < sonarTimer)
        {
            Vector3 slerpedVector = Vector3.Slerp(transform.forward, closestInteractablePos - transform.position, timer / sonarAlignTime);
            Quaternion rotation = Quaternion.LookRotation(slerpedVector, Vector3.up);
            float distToInteractable = Vector3.Distance(transform.position, closestInteractablePos);
            if (distToInteractable > distanceForMediumColor)
            {
                sonarSprite.color = Color.Lerp(mediumColor, farColor, (distToInteractable-distanceForMediumColor) / (distanceForFarColor - distanceForMediumColor));
            }
            else
            { 
                sonarSprite.color = Color.Lerp(closeColor, mediumColor, distToInteractable / distanceForMediumColor);
            }
            sonarPivotY.eulerAngles = new Vector3(sonarPivotY.eulerAngles.x, rotation.eulerAngles.y, sonarPivotY.eulerAngles.z);
            sonarPivotX.eulerAngles = new Vector3(rotation.eulerAngles.x, sonarPivotX.eulerAngles.y, sonarPivotX.eulerAngles.z);
            timer += Time.deltaTime;
            yield return null;
        }
        isSonarActive = false;
        sonar.SetActive(false);
    }
}
