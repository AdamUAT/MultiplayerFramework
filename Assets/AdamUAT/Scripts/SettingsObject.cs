using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsObject : MonoBehaviour
{
    [SerializeField]
    private SettingsManager.Settings associatedSetting;

    private void Awake()
    {
        GameManager.instance.AddSettingsObjectToSettingsManager(this);
    }

    public SettingsManager.Settings GetAssociatedSetting()
    {
        return associatedSetting;
    }

    public void UpdateAssociatedSettingValues()
    {
        GameManager.instance.GetSettingsManager().ModifySetting(gameObject, associatedSetting);
    }
}
