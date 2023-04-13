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
        
    }

    /// <summary>
    /// Sets the gameState without calling any of the related functions.
    /// </summary>
    public void SetGameStateHard(GameState newGameState)
    {
        gameState = newGameState;
    }
}
