using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIGameplay : MonoBehaviour
{
    static UIGameplay Instance;
    [SerializeField] GameObject InteractText;
    [SerializeField] GameObject FadeOutPanel;
    [SerializeField] Image FadeOutImage;
    [SerializeField] float fadeOutInDefaultTime;
    float fadeOutInCustomTime;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadedTime;
    public bool isFadingOut = false; //probably should be an enum
    public bool isFaded = false;
    public bool isFadingIn = false;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject GeneralMenuPanel;
    [SerializeField] GameObject SettingsMenuPanel;
    [SerializeField] GameObject ControlsMenuPanel;
    [SerializeField] Slider SensitivitySlider;
    [SerializeField] Slider VolumeSlider;
    [SerializeField] GameObject ShipDoc1;
    [SerializeField] GameObject ShipDoc2;
    [SerializeField] GameObject ShipDoc3;
    [SerializeField] GameObject ShipDoc4;
    [SerializeField] GameObject ShipDoc5;
    [SerializeField] Button EndGameButton;
    [SerializeField] TextMeshProUGUI TimescaleText;
    [SerializeField] TextMeshProUGUI FPSText;
    int framesSinceLastUpdate = 0;
    float timeBeforeUpdatingFPS = 0.5f;
    float fpsDisplayTimer = 0.0f;
    float trollTimer = 0.0f;
    float timeBeforeTrolling = 10.0f;
    bool docOpen = false;
    KeyCode lastHitKey;
    bool cheatsOn = false;
    [SerializeField] GameObject cheats;

    public static UIGameplay Get()
    {
        return Instance;
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        SensitivitySlider.value = SettingsData.sensitivity;
        VolumeSlider.value = SettingsData.volume;
        UpdateTimeScaleText();
    }

    private void Update()
    {
        if (docOpen && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E)))
        {
            ShipDoc1.SetActive(false);
            ShipDoc2.SetActive(false);
            ShipDoc3.SetActive(false);
            docOpen = false;
            GameplayController.Get().ChangeInputState(GameplayController.InputState.Movement);
        }
        trollTimer += Time.unscaledDeltaTime;
        fpsDisplayTimer += Time.unscaledDeltaTime;
        framesSinceLastUpdate++;
        if (fpsDisplayTimer > timeBeforeUpdatingFPS && FPSText)
        {
            if (trollTimer < timeBeforeTrolling)
            {
                FPSText.text = "FPS: " + (int)(framesSinceLastUpdate / timeBeforeUpdatingFPS);
            }
            else
            { 
                FPSText.text = "putoelquelee";
                trollTimer = 0.0f;
            }
            framesSinceLastUpdate = 0;
            fpsDisplayTimer = 0.0f;
        }
    }
    private void OnGUI()
    {
        if (!cheatsOn && Input.anyKeyDown)
        {
            if (lastHitKey == KeyCode.Minus)
            {
                if (Event.current.keyCode == KeyCode.U)
                {
                    cheatsOn = true;
                    cheats.SetActive(true);
                }
            }
            else
            {
                lastHitKey = Event.current.keyCode;
            }
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
        FadeOutPanel.SetActive(true);
        FadeOutImage.color = startFromWhite ? Color.white : Color.black;
        while (timer < fadeOutInTime)
        {
            timer += Time.deltaTime;
            FadeOutImage.color = new Color(FadeOutImage.color.r, FadeOutImage.color.g, FadeOutImage.color.b, timer / fadeOutInTime);
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
            FadeOutImage.color = new Color(FadeOutImage.color.r, FadeOutImage.color.g, FadeOutImage.color.b, 1 - (timer / fadeOutInTime));
            yield return null;
        }
        FadeOutPanel.SetActive(false);
        isFadingIn = false;
    }

    IEnumerator FadeIn()
    {
        FadeOutImage.color = Color.black;
        FadeOutPanel.SetActive(true);
        float timer = 0.0f;
        isFadingIn = true;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            FadeOutImage.color = new Color(FadeOutImage.color.r, FadeOutImage.color.g, FadeOutImage.color.b, 1 - (timer / fadeInTime));
            yield return null;
        }
        FadeOutPanel.SetActive(false);
        isFadingIn = false;
    }

    public void ShowMenu(bool shouldShow)
    {
        GameplayController.Get().OnUIMenuStateChanged();
        ChangeMenuVisibility(false);
    }

    public void ChangeMenuVisibility(bool newVisibility)
    {
        MenuPanel.SetActive(newVisibility);
        GeneralMenuPanel.SetActive(newVisibility);
        SettingsMenuPanel.SetActive(!newVisibility);
        ControlsMenuPanel.SetActive(!newVisibility);
    }

    public void UnstuckPlayer()
    {
        GameplayController.Get().UnstuckPlayer();    
    }

    public void UpdateVolume()
    {
        SettingsData.volume = VolumeSlider.value;
        AudioManager.Get().UpdateVolume();
    }

    public void UpdateCameraSensitivity()
    {
        SettingsData.sensitivity = SensitivitySlider.value;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void DisplayShipDoc1()
    {
        ShipDoc1.SetActive(true);
        OnRegularShipDocOpen();
    }

    public void DisplayShipDoc2()
    {
        ShipDoc2.SetActive(true);
        OnRegularShipDocOpen();
    }

    public void DisplayShipDoc3()
    {
        ShipDoc3.SetActive(true);
        OnRegularShipDocOpen();
    }

    void OnRegularShipDocOpen()
    { 
        docOpen = true;
        GameplayController.Get().ChangeInputState(GameplayController.InputState.UI);
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

    public void SetCustomTimeForNextFade(float customTime)
    {
        fadeOutInCustomTime = customTime;
    }

    public void IncreaseTimeScale()
    {
        Time.timeScale += 0.5f;
        UpdateTimeScaleText();
    }

    public void DecreaseTimeScale()
    {
        if (Time.timeScale > 0.0f)
        {
            Time.timeScale -= 0.5f;
            UpdateTimeScaleText();
        }
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        UpdateTimeScaleText();
    }

    public void UpdateTimeScaleText()
    {
        TimescaleText.text = "Current Speed x" + Time.timeScale;
    }
}
