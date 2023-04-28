using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //The singleton instance of the GameManager
    public static GameManager instance { get; private set; }
    //This bool is used to tell if the gameManager has been assigned it's instance value or not, since the builds have it not equal to null for some reason.

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

    //The NetworkManager cannot have any components that inherit from NetworkBehavior, so the NetworkManager is spawned as a seperate GameObject.
    //Since it is a singleton, there is no need for the GameManager to have a reference to the GameObject, just the prefab being spawned.
    [SerializeField] private GameObject networkManager;
    #endregion References

    #region Variables
    [SerializeField]
    private CustomSceneManager.Scenes startupScene = CustomSceneManager.Scenes.MainMenu;
    [SerializeField]
    private GameStateManager.GameState startupGameState = GameStateManager.GameState.TitleScreen;

    [HideInInspector]
    public List<PlayerController> players { get; set; }
    #endregion Variables

    private void Awake()
    {
        if (instance == null)
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

        //Spawn the NetworkManager
        if (networkManager != null)
        {
            Instantiate(networkManager);
        }
        else
        {
            Debug.LogError("NetworkManager prefab not assigned to GameManager.");
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

        multiplayerManager.SetLocalPlayerName("Player" + UnityEngine.Random.Range(100, 1000));

        players = new List<PlayerController>();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
