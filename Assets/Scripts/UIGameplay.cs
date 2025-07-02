using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    static UIGameplay instance;
    [SerializeField] GameObject InteractText;
    [SerializeField] GameObject fadeOutPanel;
    [SerializeField] Image fadeOutImage;
    [SerializeField] float fadeOutInDefaultTime;
    float fadeOutInCustomTime;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadedTime;
    public bool isFadingOut = false; //probably should be an enum
    public bool isFaded = false;
    public bool isFadingIn = false;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject generalMenuPanel;
    [SerializeField] GameObject settingsMenuPanel;
    [SerializeField] GameObject controlsMenuPanel;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider volumeSlider;
    [SerializeField] GameObject ShipDoc1;
    [SerializeField] GameObject ShipDoc2;
    [SerializeField] GameObject ShipDoc3;
    [SerializeField] GameObject ShipDoc4;
    [SerializeField] GameObject ShipDoc5;
    [SerializeField] Button EndGameButton;
    bool docOpen = false;

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

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        sensitivitySlider.value = SettingsData.sensitivity;
        volumeSlider.value = SettingsData.volume;
    }

    private void Update()
    {
        if (docOpen && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShipDoc1.SetActive(false);
            ShipDoc2.SetActive(false);
            ShipDoc3.SetActive(false);
            docOpen = false;
        }
    }
    public void ChangeInteractTextDisplay(bool bDisplay)
    {
        InteractText.SetActive(bDisplay);
    }

    public void FadeOutAndIn(bool startFromWhite = false) 
    {
        StartCoroutine(FadeOut(startFromWhite));
    }

    public void FadeIn(bool startFromWhite = false)
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeOut(bool startFromWhite)
    {
        float fadeOutInTime = fadeOutInCustomTime > 0 ? fadeOutInCustomTime : fadeOutInDefaultTime;
        fadeOutInCustomTime = -1.0f;
        float timer = 0.0f;
        isFadingOut = true;
        fadeOutPanel.SetActive(true);
        fadeOutImage.color = startFromWhite ? Color.white : Color.black;
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

    IEnumerator FadeIn()
    {
        fadeOutImage.color = Color.black;
        fadeOutPanel.SetActive(true);
        float timer = 0.0f;
        isFadingIn = true;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, 1 - (timer / fadeInTime));
            yield return null;
        }
        fadeOutPanel.SetActive(false);
        isFadingIn = false;
    }

    public void ShowMenu(bool shouldShow)
    {
        GameplayController.Get().OnUIMenuStateChanged();
        ChangeMenuVisibility(false);
    }

    public void ChangeMenuVisibility(bool newVisibility)
    {
        menuPanel.SetActive(newVisibility);
        generalMenuPanel.SetActive(newVisibility);
        settingsMenuPanel.SetActive(!newVisibility);
        controlsMenuPanel.SetActive(!newVisibility);
    }

    public void UpdateVolume()
    {
        SettingsData.volume = volumeSlider.value;
        AudioManager.Get().UpdateVolume();
    }

    public void UpdateCameraSensitivity()
    {
        SettingsData.sensitivity = sensitivitySlider.value;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void DisplayShipDoc1()
    {
        ShipDoc1.SetActive(true);
        docOpen = true;
    }

    public void DisplayShipDoc2()
    {
        ShipDoc2.SetActive(true);
        docOpen = true;
    }
    public void DisplayShipDoc3()
    {
        ShipDoc3.SetActive(true);
        docOpen = true;
    }
    public void DisplayShipDoc4()
    {
        GameplayController.Get().ChangeInputState(GameplayController.InputState.UI);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShipDoc4.SetActive(true);
        ShipDoc5.SetActive(false);
    }
    public void DisplayShipDoc5()
    {
        ShipDoc5.SetActive(true);
        ShipDoc4.SetActive(false);
        EndGameButton.interactable = false;
        StartCoroutine(WaitLastTimer());
    }
    IEnumerator WaitLastTimer()
    {
        yield return new WaitForSeconds(1.0f);
        EndGameButton.interactable = true;
    }
    public void StartEndingCinematic()
    {
        ShipDoc5.SetActive(false);
        GameplayController.Get().ChangeInputState(GameplayController.InputState.UI);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameplayController.Get().StartEndGameCinematic();
    }
    public void SetCustomTimeForNextFade(float customTime)
    {
        fadeOutInCustomTime = customTime;
    }
}
