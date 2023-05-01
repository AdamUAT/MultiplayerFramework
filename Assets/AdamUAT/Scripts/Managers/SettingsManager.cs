using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum Settings { MusicVolume, EffectsVolume }


    private List<SettingsObject> settingsObjects = new List<SettingsObject>();

    [SerializeField]
    private AudioMixer audioMixer;

    //Settings values
    private float musicVolume;
    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;


            if (musicVolume <= 0)
            {
                // If we are at zero, set our volume to -80 to mute it.
                audioMixer.SetFloat("Sountrack_Volume", -80);
            }
            else
            {
                //The volue is the Log10 * 20.
                audioMixer.SetFloat("Sountrack_Volume", Mathf.Log10(musicVolume) * 20);
            }
        }
    }

    private float effectsVolume;
    public float EffectsVolume
    {
        get
        {
            return effectsVolume;
        }
        set
        {
            effectsVolume = value;


            if (effectsVolume <= 0)
            {
                // If we are at zero, set our volume to -80 to mute it.
                audioMixer.SetFloat("FX_Volume", -80);
            }
            else
            {
                //The volue is the Log10 * 20.
                audioMixer.SetFloat("FX_Volume", Mathf.Log10(effectsVolume) * 20);
            }
        }
    }

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
        PlayerPrefs.SetFloat(Settings.MusicVolume.ToString(), MusicVolume);
        PlayerPrefs.SetFloat(Settings.EffectsVolume.ToString(), EffectsVolume);
    }

    /// <summary>
    /// Loads the settings from PlayerPrefs
    /// </summary>
    public void LoadSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat(Settings.MusicVolume.ToString(), 1.0f);
        EffectsVolume = PlayerPrefs.GetFloat(Settings.EffectsVolume.ToString(), 1.0f);
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
                case Settings.MusicVolume:
                    Slider musicSlider = settingsObject.GetComponent<Slider>();
                    if(musicSlider != null)
                    {
                        musicSlider.value = MusicVolume;
                    }
                    else
                    {
                        Debug.LogWarning("MusicVolume settings object does not have a slider component.");
                    }
                    break;
                case Settings.EffectsVolume:
                    Slider effectsSlider = settingsObject.GetComponent<Slider>();
                    if (effectsSlider != null)
                    {
                        effectsSlider.value = EffectsVolume;
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
            case Settings.MusicVolume:
                Slider musicSlider = option.GetComponent<Slider>();
                if(musicSlider != null)
                {
                    MusicVolume = musicSlider.value;
                }
                else
                {
                    Debug.LogWarning("MusicVolume settings object does not have a slider component.");
                }
                break;
            case Settings.EffectsVolume:
                Slider effectsSlider = option.GetComponent<Slider>();
                if (effectsSlider != null)
                {
                    EffectsVolume = effectsSlider.value;
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
