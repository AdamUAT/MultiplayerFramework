using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState { TitleScreen, MainMenu, Options, Credits }
    //The current state of the game.
    private GameState gameState;

    /// <summary>
    /// Calls functions related to the gameState being changed.
    /// </summary>
    public void ChangeGameState(GameState newGameState)
    {
        //Deactivate the UI with the old GameState.
        GameManager.instance.GetUIManager().DisableUIObjectsWithGameState(gameState);

        //Activate the UI with the new GameState.
        GameManager.instance.GetUIManager().EnableUIObjectsWithGameState(newGameState);

        //The player is loading in options, so we will load in the set options.
        if(newGameState == GameState.Options)
        {
            GameManager.instance.LoadSettings();
        }

        //The player is changing states when in the Options state.
        if(gameState == GameState.Options)
        {
            GameManager.instance.SaveSettings();
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
