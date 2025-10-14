using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject initialBrightenssButtons;
    [SerializeField] GameObject generalButtons;
    [SerializeField] GameObject settingsButtons;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Slider initialBrightnessSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider volumeSlider;
    [SerializeField] GameObject backgroundImage;
    [SerializeField] GameObject title;
    GeneralBrightnessSettings generalBrightnessSettings;
    bool firstTimeInMenu = true;

    private void Start()
    {
        generalBrightnessSettings = GeneralBrightnessSettings.Get();
        sensitivitySlider.value = SettingsData.sensitivity;
        volumeSlider.value = SettingsData.volume;
        brightnessSlider.value = generalBrightnessSettings.GetGain();
        initialBrightnessSlider.value = generalBrightnessSettings.GetGain();
        if (LevelsManager.Get() != null)
        {
            LevelsManager.Get().CleanInstance(); //jam stuff
        }
        ShowInitialBrightnessOptions(firstTimeInMenu);
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Not the first time opening main menu");
        firstTimeInMenu = false;
        ShowInitialBrightnessOptions(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SurfaceScene");
    }

    public void ShowSettings(bool show)
    {
        if (show)
        {
            brightnessSlider.value = generalBrightnessSettings.GetGain();
        }
        backgroundImage.SetActive(!show);
        generalButtons.SetActive(!show);
        settingsButtons.SetActive(show);
    }

    public void ShowCredits(bool show)
    {
        generalButtons.SetActive(!show);
        creditsPanel.SetActive(show);
    }
    public void ShowInitialBrightnessOptions(bool show)
    {
        initialBrightenssButtons.SetActive(show);
        backgroundImage.SetActive(!show);
        title.SetActive(!show);
        generalButtons.SetActive(!show);
    }

    public void UpdateVolume()
    {
        SettingsData.volume = volumeSlider.value;
        AudioManager.Get().UpdateVolume();
    }

    public void UpdateBrightness(Slider slider)
    {
        generalBrightnessSettings.UpdateGain(slider.value);
    }

    public void UpdateCameraSensitivity()
    {
        SettingsData.sensitivity = sensitivitySlider.value;
    }

    public void CloseGame()
    {
        Debug.Log("El juego se hubiese cerrado");
        Application.Quit();
    }
}
