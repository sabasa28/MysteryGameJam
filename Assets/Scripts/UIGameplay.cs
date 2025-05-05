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
    [SerializeField] float fadeOutInTime;
    [SerializeField] float fadedTime;
    public bool isFadingOut = false; //probably should be an enum
    public bool isFaded = false;
    public bool isFadingIn = false;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject generalMenuPanel;
    [SerializeField] GameObject settingsMenuPanel;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider volumeSlider;
    [SerializeField] GameObject ShipDoc1;
    [SerializeField] GameObject ShipDoc2;
    [SerializeField] GameObject ShipDoc3;
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

    IEnumerator FadeOut(bool startFromWhite = false)
    {
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
    }

    public void UpdateVolume()
    {
        SettingsData.volume = volumeSlider.value;
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
}
