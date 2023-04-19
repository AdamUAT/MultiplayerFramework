using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<UIObject> uiObjects = new List<UIObject>();

    /// <summary>
    /// Tells the UIManager that a new canvas UI object is in the scene.
    /// </summary>
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
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void MainMenuToOptions()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Options);
    }
    public void OptionsToMainMenu()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void MainMenuToCredits()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Credits);
    }
    public void CreditsToMainMenu()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void Quit()
    {
        GameManager.instance.QuitGame();
    }
    //Since it directly goes from MainMenu to Gameplay, it is singleplayer.
    public void MainMenuToGameplay()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Gameplay);
    }
    #endregion UI Element Functions
}
