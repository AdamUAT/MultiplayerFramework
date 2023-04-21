using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsObject : UIObject
{
    [SerializeField]
    private SettingsManager.Settings associatedSetting;

    protected override void Awake()
    {
        GameManager.instance.settingsManager.AddSettingsObject(this);
        base.Awake();
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
