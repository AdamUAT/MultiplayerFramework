using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<UIObject> uiObjects = new List<UIObject>();

    public void AddUIObject(UIObject uiObject)
    {
        uiObjects.Add(uiObject);
    }

    /// <summary>
    /// Disables all the UI objects in the current scene.
    /// </summary>
    public void DisableAllUIObjects()
    {
        foreach(UIObject uiObject in uiObjects)
        {
            uiObject.DisableUIObject();
        }
    }

    /// <summary>
    /// Disables all the UI objects in the current scene with a specifide gameState.
    /// </summary>
    public void DisableUIObjectsWithGameState(GameStateManager.GameState gameState)
    {
        foreach (UIObject uiObject in uiObjects)
        {
            uiObject.DisableUIObject(gameState);
        }
    }

    /// <summary>
    /// Enables all the UI objects in the current scene with a specifide gameState.
    /// </summary>
    public void EnableUIObjectsWithGameState(GameStateManager.GameState gameState)
    {
        foreach (UIObject uiObject in uiObjects)
        {
            uiObject.EnableUIObject(gameState);
        }
    }

    #region UI Element Functions
    public void TitleToMainMenu()
    {
        GameManager.instance.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void MainMenuToOptions()
    {
        GameManager.instance.ChangeGameState(GameStateManager.GameState.Options);
    }
    public void OptionsToMainMenu()
    {
        GameManager.instance.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void MainMenuToCredits()
    {
        GameManager.instance.ChangeGameState(GameStateManager.GameState.Credits);
    }
    public void CreditsToMainMenu()
    {
        GameManager.instance.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void Quit()
    {
        GameManager.instance.QuitGame();
    }
    #endregion UI Element Functions
}
