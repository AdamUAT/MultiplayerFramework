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

    //The manager that allows the pawn GameObject to be passed through Rpc.
    public PawnPrefabsManager pawnPrefabsManager { get; private set; }

    //The manager that controls the camera(s).
    public CameraManager cameraManager { get; private set; }

    //The manager that controls all of the player's input keybinds.
    public InputManager inputManager { get; private set; }

    //The manager that controlls all the network related stuff
    public MultiplayerManager multiplayerManager { get; private set; }
    [SerializeField] private GameObject multiplayerManagerReference;

    //The NetworkManager cannot have any components that inherit from NetworkBehaviour, so the NetworkManager is spawned as a seperate GameObject.
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

    private bool isPaused;
    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
        }
    }
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

        //Assign the PawnPrefabsManager
        pawnPrefabsManager = GetComponent<PawnPrefabsManager>();
        if (pawnPrefabsManager == null)
        {
            pawnPrefabsManager = gameObject.AddComponent<PawnPrefabsManager>();
        }

        cameraManager = GetComponent<CameraManager>();
        if(cameraManager == null)
        {
            cameraManager = gameObject.AddComponent<CameraManager>();
        }

        inputManager = GetComponent<InputManager>();
        if(inputManager == null)
        {
            inputManager = gameObject.AddComponent<InputManager>();
        }

        //Assign the multiplayerManager
        //Since the multiplayerManager inherits from NetworkBehavior instead of MonoBehavior, it needs it's own gameObject.
        if (multiplayerManagerReference != null)
        {
            multiplayerManager = Instantiate(multiplayerManagerReference).GetComponent<MultiplayerManager>();
            DontDestroyOnLoad (multiplayerManager);
        }
        else
        {
            Debug.LogError("No multiplayerManager prefab assigned.");
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

        pawnPrefabsManager.CompileDictionary();

        cameraManager.CurrentCamera = Camera.main;
        if(cameraManager.CurrentCamera == null)
        {
            //If there were no cameras in the scene, then this will create one.
            GameObject newCamera = new GameObject("InstantiatedCamera");
            cameraManager.CurrentCamera = newCamera.AddComponent<Camera>();
        }

        players = new List<PlayerController>();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void Update()
    {
        if(multiplayerManager.IsClientHost())
        {
            multiplayerManager.UpdatePlayersClientRpc();
        }


        //Update all the AIs in the scene.
    }
}
