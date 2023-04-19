using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsObject : MonoBehaviour
{
    [SerializeField]
    private SettingsManager.Settings associatedSetting;

    private void Awake()
    {
        GameManager.instance.settingsManager.AddSettingsObject(this);
    }

    public SettingsManager.Settings GetAssociatedSetting()
    {
        return associatedSetting;
    }

    public void UpdateAssociatedSettingValues()
    {
        GameManager.instance.settingsManager.ModifySetting(gameObject, associatedSetting);
    }
}
