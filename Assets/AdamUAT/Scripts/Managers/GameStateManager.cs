using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;

public class GameStateManager : MonoBehaviour
{
    public enum GameState { TitleScreen, MainMenu, Options, Credits, Gameplay, HostOrJoin, Join, Host, Lobby }
    //The current state of the game.
    public GameState currentGameState { get; private set; }

    /// <summary>
    /// Changes the GameState of the game and calls any of the related functions.
    /// </summary>
    public void ChangeGameState(GameState newGameState)
    {
        //Deactivate the UI with the old GameState.
        GameManager.instance.uiManager.DisableUIObjectsWithGameState(currentGameState);

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
        if (currentGameState == GameState.Options)
        {
            GameManager.instance.settingsManager.SaveSettings();
        }

        //If it transitions directly from MainMenu to gameplay, it is singleplayer, since multiplayer goes through several other game states.
        if(currentGameState == GameState.MainMenu && newGameState == GameState.Gameplay)
        {
            //A server must be created, even in singleplayer, for the game to work. 
            GameManager.instance.multiplayerManager.StartSinglePlayer();

            GameManager.instance.sceneManager.ChangeScene(CustomSceneManager.Scenes.Gameplay);
        }

        //If it transitions from MainMenu to HostOrJoin, then load the lobby scene.
        if(currentGameState == GameState.MainMenu && newGameState == GameState.HostOrJoin)
        {
            GameManager.instance.sceneManager.ChangeScene(CustomSceneManager.Scenes.Lobby);
        }

        //Changes scenes back the MainMenu from HostOrJoin
        if(currentGameState == GameState.HostOrJoin && newGameState == GameState.MainMenu)
        {
            GameManager.instance.sceneManager.ChangeScene(CustomSceneManager.Scenes.MainMenu);
        }

        //This means the game has started.
        if(currentGameState == GameState.Lobby && newGameState == GameState.Gameplay)
        {
            //Close the lobby so no one else can join the game.
            GameManager.instance.multiplayerManager.DeleteLobby();
        }

        //This means the player exited the lobby they were in.
        if(currentGameState == GameState.Lobby && newGameState == GameState.HostOrJoin)
        {
            //Remove the player from the lobby.
            //Task.WhenAll(GameManager.instance.multiplayerManager.LeaveLobby());
            GameManager.instance.multiplayerManager.LeaveLobby();
        }

        currentGameState = newGameState;
    }

    /// <summary>
    /// Sets the currentGameState without calling any of the related functions.
    /// </summary>
    public void SetGameStateHard(GameState newGameState)
    {
        currentGameState = newGameState;
    }
}
