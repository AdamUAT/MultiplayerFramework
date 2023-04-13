using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    //The singleton instance of the GameManager
    public static GameManager instance;

    #region References
    //A reference to the script that controlls all of the game's state changes.
    private GameStateManager gameState;

    //The manager that controlls all of the UI for the game.
    private UIManager uiManager;

    #endregion References

    private void Awake()
    {
        if(instance == null)
        {
            //Make Singleton
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Assign references first thing in the game.
            AssignReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeVariables();
    }

    /// <summary>
    /// Assign any references, so any variables in those references can be initialized in Start().
    /// </summary>
    private void AssignReferences()
    {
        //Assign the GameStateManager.
        gameState = GetComponent<GameStateManager>();
        if (gameState == null)
        {
            gameState = gameObject.AddComponent<GameStateManager>();
        }
    }

    /// <summary>
    /// Initialize any variables in the game.
    /// </summary>
    private void InitializeVariables()
    {
        //Starts the game in the TitleScreen.
        gameState.SetGameStateHard(GameStateManager.GameState.TitleScreen);
    }

    /// <summary>
    /// Changes the GameState of the game and calls any of the related functions.
    /// </summary>
    public void ChangeGameState(GameStateManager.GameState newGameState)
    {
        gameState.ChangeGameState(newGameState);
    }
}