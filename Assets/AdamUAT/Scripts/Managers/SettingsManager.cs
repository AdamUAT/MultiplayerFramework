using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum Settings { musicVolume, effectsVolume }


    private List<SettingsObject> settingsObjects = new List<SettingsObject>();

    //Settings values
    public float musicVolume { get; set; }
    private float effectsVolume { get; set; }

    /// <summary>
    /// Tells the SettingsManager that a new settingsObject is in the scene.
    /// </summary>
    public void AddSettingsObject(SettingsObject settingsObject)
    {
        settingsObjects.Add(settingsObject);
    }

    /// <summary>
    /// Saves the current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(Settings.musicVolume.ToString(), musicVolume);
        PlayerPrefs.SetFloat(Settings.effectsVolume.ToString(), effectsVolume);
    }

    /// <summary>
    /// Loads the settings from PlayerPrefs
    /// </summary>
    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(Settings.musicVolume.ToString(), 1.0f);
        effectsVolume = PlayerPrefs.GetFloat(Settings.effectsVolume.ToString(), 1.0f);
    }

    /// <summary>
    /// Updates every UI element to match the values stored.
    /// </summary>
    public void UpdateSettings()
    {
        foreach(SettingsObject settingsObject in settingsObjects)
        {
            switch(settingsObject.GetAssociatedSetting())
            {
                case Settings.musicVolume:
                    Slider musicSlider = settingsObject.GetComponent<Slider>();
                    if(musicSlider != null)
                    {
                        musicSlider.value = musicVolume;
                    }
                    else
                    {
                        Debug.LogWarning("MusicVolume settings object does not have a slider component.");
                    }
                    break;
                case Settings.effectsVolume:
                    Slider effectsSlider = settingsObject.GetComponent<Slider>();
                    if (effectsSlider != null)
                    {
                        effectsSlider.value = effectsVolume;
                    }
                    else
                    {
                        Debug.LogWarning("EffectsVolume settings object does not have a slider component.");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Makes the UI element specified match the settings stored.
    /// </summary>
    public void ModifySetting(GameObject option, Settings associatedSetting)
    {
        switch (associatedSetting)
        {
            case Settings.musicVolume:
                Slider musicSlider = option.GetComponent<Slider>();
                if(musicSlider != null)
                {
                    musicVolume = musicSlider.value;
                }
                else
                {
                    Debug.LogWarning("MusicVolume settings object does not have a slider component.");
                }
                break;
            case Settings.effectsVolume:
                Slider effectsSlider = option.GetComponent<Slider>();
                if (effectsSlider != null)
                {
                    effectsVolume = effectsSlider.value;
                }
                else
                {
                    Debug.LogWarning("EffectsVolume settings object does not have a slider component.");
                }
                break;
            default:
                Debug.LogWarning("Settings enum went out of bounds in SettingsManager UpdateSetting.");
                break;
        }
    }
}