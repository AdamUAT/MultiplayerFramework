using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameStates this UI object will be used in.")]
    private List<GameStateManager.GameState> gameState;

    protected virtual void Awake()
    {
        GameManager.instance.uiManager.AddUIObject(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.uiManager.RemoveUIObject(this);
    }

    /// <summary>
    /// Disables this UIObject
    /// </summary>
    public void DisableUIObject()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Disables this UIObject if it has the specified GameState.
    /// </summary>
    public void DisableUIObject(GameStateManager.GameState associatedGameState)
    {
        //Loops through each GameState this UIObject is associated with and checks if associatedGameState is one of them.
        foreach(GameStateManager.GameState state in gameState)
        {
            if (state == associatedGameState)
            {
                gameObject.SetActive(false);
                return; //Stops the loop
            }
        }
    }

    /// <summary>
    /// Enables this UIObject
    /// </summary>
    public void EnableUIObject()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enables this UIObject if it has the specified GameState.
    /// </summary>
    public void EnableUIObject(GameStateManager.GameState associatedGameState)
    {
        //Loops through each GameState this UIObject is associated with and checks if associatedGameState is one of them.
        foreach (GameStateManager.GameState state in gameState)
        {
            if (state == associatedGameState)
            {
                gameObject.SetActive(true);
                return; //Stops the loop
            }
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
    //This changes scenes to Lobby
    public void MainMenuToHostOrJoin()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);
    }
    public void HostOrJoinToHost()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Host);
    }
    public void HostOrJoinToJoin()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Join);
    }
    public void HostOrJoinToMainMenu()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.MainMenu);
    }
    public void HostToHostOrJoin()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);
    }
    public void JoinToHostOrJoin()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);
    }
    public void LobbyToHostOrJoin()
    {
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);
    }
    #endregion UI Element Functions
}
