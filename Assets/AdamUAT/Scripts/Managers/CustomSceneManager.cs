using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    //The enum MUST have values that corrospond to the actual names of the scenes.
    public enum Scenes { MainMenu, Lobby, Gameplay}
    //The current scene of the game.
    private Scenes currentScene;

    /// <summary>
    /// Calls functions related to the scene being changed.
    /// </summary>
    public void ChangeScene(Scenes newScene)
    {
        //Loads the scene.
        if (IsSceneValid(newScene.ToString()))
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(newScene.ToString()))
            {
                currentScene = newScene;
                SceneManager.LoadScene(newScene.ToString());

                //Initialize the UI in the scene when it finishes loading.
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Debug.LogWarning("The scene passed into SetSceneHard in CustomSceneManager is the active scene, so it did not reload.");
            }
        }
        else
        {
            Debug.LogError("The scene passed into SetSceneHard in CustomSceneManager is invalid and the scene did not load.");
        }
    }
    
    /// <summary>
    /// Changes the scene on all clients and calls functions related to the scene being changed.
    /// </summary>
    public void ChangeSceneNetwork(Scenes newScene)
    {
        //Loads the scene.
        if (IsSceneValid(newScene.ToString()))
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(newScene.ToString()))
            {
                currentScene = newScene;
                NetworkManager.Singleton.SceneManager.LoadScene(newScene.ToString(), LoadSceneMode.Single);

                //Initialize the UI in the scene when it finishes loading.
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Debug.LogWarning("The scene passed into SetSceneHard in CustomSceneManager is the active scene, so it did not reload.");
            }
        }
        else
        {
            Debug.LogError("The scene passed into SetSceneHard in CustomSceneManager is invalid and the scene did not load.");
        }
    }

    /// <summary>
    /// Function that handles calling all the other functions when a scene finishes changing.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Unassignes the delegate when this function is called so it is only called once.
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //Initializes the UI for this scene.
        //If the NetworkManager is active, it does the ClientRpc version.
        if (GameManager.instance.multiplayerManager.isMultiplayer)
            GameManager.instance.multiplayerManager.InitializeUIClientRpc();
        else
            InitializeUI();
    }

    /// <summary>
    /// Changes the active scene without calling any of the other functions.
    /// </summary>
    public void SetSceneHard(Scenes newScene)
    {
        currentScene = newScene;

        //Loads the scene.
        if(IsSceneValid(newScene.ToString()))
        {
            if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName(newScene.ToString()))
            {
                SceneManager.LoadScene(newScene.ToString());
                SceneManager.sceneLoaded += OnSceneLoadedHard;
            }
            else
            {
                Debug.LogWarning("The scene passed into SetSceneHard in CustomSceneManager is the active scene, so it did not reload.");
            }
        }
        else
        {
            Debug.LogError("The scene passed into SetSceneHard in CustomSceneManager is invalid and the scene did not load.");
        }
    }

    /// <summary>
    /// Function that handles calling all the necessary functions for a hard set scene.
    /// </summary>
    private void OnSceneLoadedHard(Scene scene, LoadSceneMode mode)
    {
        //Unassignes the delegate when this function is called so it is only called once.
        SceneManager.sceneLoaded -= OnSceneLoadedHard;

        //Initializes the UI for this scene.
        if (GameManager.instance.multiplayerManager.isMultiplayer)
            GameManager.instance.multiplayerManager.InitializeUIClientRpc();
        else
            InitializeUI();
    }

    /// <summary>
    /// Checks if the provided name of a scene is a valid scene to load.
    /// </summary>
    private bool IsSceneValid(string sceneName)
    {
        //Checks every scene in the build if it exists.
        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            //If the scene provided matches the scene in the build, it is valid.
            //We have to use System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)) because the methods in SceneManager only returns the name of the active scene and null for every other scene.
            if (Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)) == sceneName)
            {
                return true;
            }
        }

        //If none of the scenes match, then the scene is not valid.
        return false;
    }

    /// <summary>
    /// Disables all the UI except the one that should be showing.
    /// </summary>
    public void InitializeUI()
    {
        GameManager.instance.uiManager.DisableAllUIObjects();

        GameManager.instance.uiManager.EnableUIObjectsWithGameState(GameManager.instance.gameStateManager.currentGameState);
    }
}
