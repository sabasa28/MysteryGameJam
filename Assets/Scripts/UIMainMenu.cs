using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject generalButtons;
    [SerializeField] GameObject settingsButtons;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        sensitivitySlider.value = SettingsData.sensitivity;
        volumeSlider.value = SettingsData.volume;
        if (LevelsManager.Get() != null)
        {
            LevelsManager.Get().CleanInstance(); //jam stuff
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SurfaceScene");
    }

    public void ShowSettings(bool show)
    {
        generalButtons.SetActive(!show);
        settingsButtons.SetActive(show);
    }

    public void ShowCredits(bool show)
    {
        generalButtons.SetActive(!show);
        creditsPanel.SetActive(show);
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

    public void CloseGame()
    {
        Debug.Log("El juego se hubiese cerrado");
        Application.Quit();
    }
}
