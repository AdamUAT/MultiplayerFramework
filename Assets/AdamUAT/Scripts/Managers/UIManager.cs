using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<UIObject> uiObjects;

    public void AddUIObject(UIObject uiObject)
    {
        uiObjects.Add(uiObject);
    }
}
