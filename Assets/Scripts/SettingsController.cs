using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private int defaultResolutionIndex = 0;

    private const int DEFAULT_WIDTH = 1920;
    private const int DEFAULT_HEIGHT = 1080;
    private const float DEFAULT_VOLUME = 0.5f;
    private const bool DEFAULT_FULLSCREEN = true;

    [Serializable]
    private class SettingsData
    {
        public int resolutionIndex;
        public float masterVolume;
        public bool isFullscreen;
    }

    private SettingsData settings = new SettingsData();
    private string settingsFileName = "settings.json";

    private string SettingsPath =>
        Path.Combine(Application.persistentDataPath, settingsFileName);

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        defaultResolutionIndex = FindDefaultResolutionIndex();

        LoadSettings();

        if (settings.resolutionIndex < 0 || settings.resolutionIndex >= resolutions.Length)
            settings.resolutionIndex = defaultResolutionIndex;

        if (settings.masterVolume < 0f || settings.masterVolume > 1f)
            settings.masterVolume = DEFAULT_VOLUME;

        resolutionDropdown.value = settings.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false;
        volumeSlider.value = settings.masterVolume;

        fullscreenToggle.isOn = settings.isFullscreen;

        ApplyResolution(settings.resolutionIndex, settings.isFullscreen);
        ApplyVolume(settings.masterVolume);
        Screen.fullScreen = settings.isFullscreen;
    }

    private int FindDefaultResolutionIndex()
    {
        int indexOfCurrent = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];

            if (r.width == Screen.currentResolution.width &&
                r.height == Screen.currentResolution.height)
            {
                indexOfCurrent = i;
            }

            if (r.width == DEFAULT_WIDTH && r.height == DEFAULT_HEIGHT)
            {
                return i;
            }
        }

        return indexOfCurrent;
    }

    // === Handlers ===

    public void OnResolutionChanged(int index)
    {
        settings.resolutionIndex = index;
        ApplyResolution(index, settings.isFullscreen);
        SaveSettings();
    }

    public void OnVolumeChanged(float value)
    {
        settings.masterVolume = value;
        ApplyVolume(value);
        SaveSettings();
    }

    public void OnFullscreenToggleChanged(bool isOn)
    {
        settings.isFullscreen = isOn;
        Screen.fullScreen = isOn;
        ApplyResolution(settings.resolutionIndex, isOn);
        SaveSettings();
    }


    private void ApplyResolution(int index, bool fullscreen)
    {
        if (index < 0 || index >= resolutions.Length) return;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, fullscreen);
    }

    private void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;
    }


    private void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(SettingsPath, json);
            Debug.Log("Settings saved: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }
    }

    private void LoadSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            settings.resolutionIndex = defaultResolutionIndex;
            settings.masterVolume = DEFAULT_VOLUME;
            settings.isFullscreen = DEFAULT_FULLSCREEN;

            SaveSettings();
            return;
        }

        try
        {
            string json = File.ReadAllText(SettingsPath);
            settings = JsonUtility.FromJson<SettingsData>(json);

            if (settings == null)
            {
                settings = new SettingsData();
                settings.resolutionIndex = defaultResolutionIndex;
                settings.masterVolume = DEFAULT_VOLUME;
                settings.isFullscreen = DEFAULT_FULLSCREEN;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load settings: " + e.Message);
            settings.resolutionIndex = defaultResolutionIndex;
            settings.masterVolume = DEFAULT_VOLUME;
            settings.isFullscreen = DEFAULT_FULLSCREEN;
        }
    }
}
