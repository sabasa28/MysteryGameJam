using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    static UIGameplay instance;
    [SerializeField] GameObject InteractText;
    [SerializeField] GameObject playerDocs;
    bool playerDocsActive;
    [SerializeField] GameObject fadeOutPanel;
    [SerializeField] Image fadeOutImage;
    [SerializeField] float fadeOutInTime;
    [SerializeField] float fadedTime;
    public bool isFadingOut = false; //probably should be an enum
    public bool isFaded = false;
    public bool isFadingIn = false;
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

    public void FadeOutAndIn() 
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = 0.0f;
        isFadingOut = true;
        fadeOutPanel.SetActive(true);
        while (timer < fadeOutInTime)
        {
            timer += Time.deltaTime;
            fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, timer / fadeOutInTime);
            yield return null;
        }
        isFadingOut = false;
        isFaded = true;
        yield return new WaitForSeconds(fadedTime);
        isFaded = false;
        isFadingIn = true;
        timer = 0.0f;
        while (timer < fadeOutInTime)
        {
            timer += Time.deltaTime;
            fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, 1 - (timer / fadeOutInTime));
            yield return null;
        }
        fadeOutPanel.SetActive(false);
        isFadingIn = false;
    }
}
