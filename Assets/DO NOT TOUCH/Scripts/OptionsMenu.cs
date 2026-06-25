using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [Header("Display Settings")]
    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle vSyncToggle;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    private Resolution[] resolutions;
    private bool isLoading;

    private void Start()
    {
        isLoading = true;

        SetupScreenModes();
        SetupResolutions();
        LoadSettings();

        isLoading = false;
    }

    #region DISPLAY

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length)
            return;

        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreenMode
        );

        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    private void SetupScreenModes()
    {
        screenModeDropdown.ClearOptions();

        screenModeDropdown.AddOptions(new List<string>()
        {
            "Borderless Fullscreen",
            "Windowed",
            "Exclusive Fullscreen"
        });
    }

    public void SetScreenMode(int index)
    {
        FullScreenMode mode = FullScreenMode.FullScreenWindow;

        switch (index)
        {
            case 0:
                mode = FullScreenMode.FullScreenWindow;
                break;

            case 1:
                mode = FullScreenMode.Windowed;
                break;

            case 2:
                mode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        Screen.fullScreenMode = mode;

        PlayerPrefs.SetInt("ScreenMode", index);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;

        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    #endregion

    #region AUDIO

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);

        PlayerPrefs.SetFloat("MasterVolume", value);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);

        PlayerPrefs.SetFloat("SFXVolume", value);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);

        PlayerPrefs.SetFloat("MusicVolume", value);

        if (!isLoading)
            PlayerPrefs.Save();
    }

    private void SetVolume(string parameter, float value)
    {
        value = Mathf.Clamp(value, 0.001f, 1f);

        float db = Mathf.Log10(value) * 20f;

        bool success = audioMixer.SetFloat(parameter, db);

        if (!success)
        {
            Debug.LogWarning($"Audio Mixer parameter '{parameter}' not found!");
        }
    }

    #endregion

    #region SAVE / LOAD

    private void LoadSettings()
    {
        LoadResolution();
        LoadScreenMode();
        LoadVSync();

        LoadVolume("MasterVolume", masterSlider, SetMasterVolume);
        LoadVolume("SFXVolume", sfxSlider, SetSFXVolume);
        LoadVolume("MusicVolume", musicSlider, SetMusicVolume);
    }

    private void LoadResolution()
    {
        int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);

        int resolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width &&
                resolutions[i].height == height)
            {
                resolutionIndex = i;
                break;
            }
        }

        resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
        SetResolution(resolutionIndex);
    }

    private void LoadScreenMode()
    {
        int modeIndex = PlayerPrefs.GetInt("ScreenMode", 0);

        screenModeDropdown.SetValueWithoutNotify(modeIndex);
        SetScreenMode(modeIndex);
    }

    private void LoadVSync()
    {
        bool enabled = PlayerPrefs.GetInt("VSync", 1) == 1;

        vSyncToggle.SetIsOnWithoutNotify(enabled);
        SetVSync(enabled);
    }

    private void LoadVolume(string key, Slider slider, System.Action<float> setter)
    {
        float value = PlayerPrefs.GetFloat(key, 1f);

        slider.SetValueWithoutNotify(value);
        setter(value);
    }

    #endregion
}