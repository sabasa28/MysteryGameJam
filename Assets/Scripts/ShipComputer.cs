using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipComputer : MonoBehaviour, IInteractable
{
    [SerializeField] Transform InteractingCameraPos;
    [SerializeField] float timeMovingCamera;
    [SerializeField] float timeReturningCamera;
    bool isBeingLookedAt = false;
    bool isCheckingPassword = false;
    string insertedString = "";
    [SerializeField] TextMeshProUGUI insertedText;
    [SerializeField] string correctPassword;
    int passwordLength;
    [SerializeField] PasswordChecker passwordChecker;
    [SerializeField] AudioClip[] keySounds;
    [SerializeField] AudioClip backspaceKeySound;
    [SerializeField] float keySoundVolume;

    private void Start()
    {
        passwordLength = correctPassword.Length;
        UpdateText();
    }
    public void Interact()
    {
        GameplayController.Get().MovePlayerCameraAndReturn(InteractingCameraPos, timeMovingCamera, timeReturningCamera);
        StartCoroutine(WaitForZoomInAndTakeInput());
        insertedString = "";
        UpdateText();
    }

    public bool IsInteractable()
    {
        return !GameplayController.Get().TabletEnabled();
    }

    public void RemoveFromNecessaryInteractables()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (isBeingLookedAt && !isCheckingPassword)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StopBeingLookedAt();
                insertedString = "";
                UpdateText();
            }

            if (insertedString.Length < passwordLength)
            {
                for (int i = (int)KeyCode.Alpha0; i < (int)KeyCode.Alpha0+10; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i))
                    {
                        insertedString += i - (int)KeyCode.Alpha0;
                        PlayRandomKeySound();
                        UpdateText();
                    }
                }
            }

            if (insertedString.Length > 0 && Input.GetKeyDown(KeyCode.Backspace))
            {
                insertedString = insertedString.Remove(insertedString.Length - 1);
                AudioManager.Get().PlaySFX(backspaceKeySound, keySoundVolume);
                UpdateText();
            }
        }
    }

    void UpdateText()
    {
        insertedText.text = "";
        for (int i = 0; i < insertedString.Length; i++)
        {
            insertedText.text += insertedString[i] + " ";
        }

        for (int i = 0; i < passwordLength - insertedString.Length; i++)
        {
            insertedText.text += "_ ";
        }

        if (insertedString.Length == passwordLength) StartCoroutine(CheckPasswordIsCorrect());
    }

    IEnumerator CheckPasswordIsCorrect()
    {
        bool passwordIsCorrect = insertedString == correctPassword;
        isCheckingPassword = true;
        if (passwordIsCorrect)
        {
            passwordChecker.SuccessfulPassword();
        }
        else
        {
            passwordChecker.FailedPassword();
        }
        yield return new WaitUntil(() => passwordChecker.idle);
        isCheckingPassword = false;
        if (passwordIsCorrect)
        {
            StopBeingLookedAt();
            GameplayController.Get().EnableAndOpenTablet();
        }
        else
        {
            insertedString = "";
            UpdateText();
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

    void PlayRandomKeySound()
    {
        if (keySounds.Length == 0) return;

        int randomNum = Random.Range(0,keySounds.Length);
        AudioManager.Get().PlaySFX(keySounds[randomNum], keySoundVolume);
    }
}
