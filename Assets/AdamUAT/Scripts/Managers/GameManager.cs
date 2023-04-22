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
    public GameStateManager gameStateManager { get; private set; }

    //The manager that controlls all of the UI for the game.
    public UIManager uiManager { get; private set; }

    //The manager that controlls the scene-related stuff.
    public CustomSceneManager sceneManager { get; private set; }

    //The manager that controlls all the settings for the game.
    public SettingsManager settingsManager { get; private set; }

    //The manager that controlls all the network related stuff
    public MultiplayerManager multiplayerManager { get; private set; }
    #endregion References

    #region Variables
    [SerializeField]
    private CustomSceneManager.Scenes startupScene = CustomSceneManager.Scenes.MainMenu;
    [SerializeField]
    private GameStateManager.GameState startupGameState = GameStateManager.GameState.TitleScreen;
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
        gameStateManager = GetComponent<GameStateManager>();
        if (gameStateManager == null)
        {
            gameStateManager = gameObject.AddComponent<GameStateManager>();
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

        //Assign the multiplayerManager
        multiplayerManager = GetComponent<MultiplayerManager>();
        if(multiplayerManager == null)
        {
            multiplayerManager = gameObject.AddComponent<MultiplayerManager>(); 
        }
    }

    /// <summary>
    /// Initialize any variables in the game.
    /// </summary>
    private void InitializeVariables()
    {
        //Starts the game in the TitleScreen.
        gameStateManager.SetGameStateHard(startupGameState);

        //Sets the starting scene to the MainMenu.
        sceneManager.SetSceneHard(startupScene);

        uiManager.DisableAllUIObjects();

        uiManager.EnableUIObjectsWithGameState(startupGameState);

        //Load the settings from PlayerPrefs so the settings persist from the last game session.
        settingsManager.LoadSettings();

        multiplayerManager.PrimeRelay();

        multiplayerManager.playerName = "Player" + UnityEngine.Random.Range(100, 1000);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
