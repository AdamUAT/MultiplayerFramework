using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState { TitleScreen, MainMenu, Options, Credits, Gameplay }
    //The current state of the game.
    private GameState gameState;

    /// <summary>
    /// Changes the GameState of the game and calls any of the related functions.
    /// </summary>
    public void ChangeGameState(GameState newGameState)
    {
        //Deactivate the UI with the old GameState.
        GameManager.instance.uiManager.DisableUIObjectsWithGameState(gameState);

        //Activate the UI with the new GameState.
        GameManager.instance.uiManager.EnableUIObjectsWithGameState(newGameState);

        // Loads the set settings from PlayerPrefs whenever the player pulls up the options screen.
        if (newGameState == GameState.Options)
        {
            //Loads the values stored in PlayerPrefs into the values stored in SettingsManager so they can be accessed during gameplay.
            GameManager.instance.settingsManager.LoadSettings();

            //Update the current values of all the ui to match the current settings.
            GameManager.instance.settingsManager.UpdateSettings();
        }

        // Saves the set settings to PlayerPrefs whenever the player closes the options screen.
        if (gameState == GameState.Options)
        {
            GameManager.instance.settingsManager.SaveSettings();
        }

        //If it transitions directly from MainMenu to gameplay, it is singleplayer, since multiplayer goes through several other game states.
        if(gameState == GameState.MainMenu && newGameState == GameState.Gameplay)
        {
            //A server must be created, even in singleplayer, for the game to work. 
            GameManager.instance.multiplayerManager.StartSinglePlayer();

            GameManager.instance.sceneManager.ChangeScene(CustomSceneManager.Scenes.Gameplay);
        }

        gameState = newGameState;
    }

    /// <summary>
    /// Sets the gameState without calling any of the related functions.
    /// </summary>
    public void SetGameStateHard(GameState newGameState)
    {
        gameState = newGameState;
    }
}
