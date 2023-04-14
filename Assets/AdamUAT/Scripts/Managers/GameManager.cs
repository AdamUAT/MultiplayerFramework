using System;
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

    //The manager that controlls the scene-related stuff.
    private CustomSceneManager sceneManager;

    //The manager that controlls all the settings for the game.
    private SettingsManager settingsManager;
    #endregion References

    #region Variables
    [SerializeField]
    private CustomSceneManager.Scenes startupScene = CustomSceneManager.Scenes.MainMenu;
    #endregion Variables

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

        //Assign the UIManager
        uiManager = GetComponent<UIManager>();
        if (uiManager == null)
        {
            uiManager = gameObject.AddComponent<UIManager>();
        }

        //Assign the SceneManager
        sceneManager = GetComponent<CustomSceneManager>();
        if (sceneManager == null)
        {
            sceneManager = gameObject.AddComponent<CustomSceneManager>();
        }

        //Assign the SettingsManager
        settingsManager = GetComponent<SettingsManager>();
        if (settingsManager == null)
        {
            settingsManager = gameObject.AddComponent<SettingsManager>();
        }
    }

    /// <summary>
    /// Initialize any variables in the game.
    /// </summary>
    private void InitializeVariables()
    {
        //Starts the game in the TitleScreen.
        gameState.SetGameStateHard(GameStateManager.GameState.TitleScreen);

        //Sets the starting scene to the MainMenu.
        sceneManager.SetSceneHard(startupScene);

        uiManager.DisableAllUIObjects();

        uiManager.EnableUIObjectsWithGameState(GameStateManager.GameState.TitleScreen);

        //Load the settings from PlayerPrefs so the settings persist from the last game session.
        settingsManager.LoadSettings();
    }

    /// <summary>
    /// Changes the GameState of the game and calls any of the related functions.
    /// </summary>
    public void ChangeGameState(GameStateManager.GameState newGameState)
    {
        gameState.ChangeGameState(newGameState);
    }

    /// <summary>
    /// Tells the UIManager that a new canvas UI object is in the scene.
    /// </summary>
    public void AddUIObjectToUIManager(UIObject uiObject)
    {
        uiManager.AddUIObject(uiObject);
    }

    public UIManager GetUIManager()
    {
        return uiManager;
    }

    public SettingsManager GetSettingsManager()
    {
        return settingsManager;
    }

    /// <summary>
    /// Tells the SettingsManager that a new settingsObject is in the scene.
    /// </summary>
    public void AddSettingsObjectToSettingsManager(SettingsObject settingsObject)
    {
        settingsManager.AddSettingsObject(settingsObject);
    }

    /// <summary>
    /// Loads the set settings from PlayerPrefs whenever the player pulls up the options screen.
    /// </summary>
    public void LoadSettings()
    {
        //Loads the values stored in PlayerPrefs into the values stored in SettingsManager so they can be accessed during gameplay.
        settingsManager.LoadSettings();

        //Update the current values of all the ui to match the current settings.
        settingsManager.UpdateSettings();
    }

    /// <summary>
    /// Saves the set settings to PlayerPrefs whenever the player closes the options screen.
    /// </summary>
    public void SaveSettings()
    {
        settingsManager.SaveSettings();
    }
}
