using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    //The enum MUST have values that corrospond to the actual names of the scenes.
    public enum Scenes { MainMenu, Gameplay}
    //The current scene of the game.
    private Scenes currentScene;

    /// <summary>
    /// Calls functions related to the scene being changed.
    /// </summary>
    public void ChangeScene(Scenes newScene)
    {


        currentScene = newScene;


        //Loads the scene.
        if (IsSceneValid(newScene.ToString()))
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(newScene.ToString()))
            {
                SceneManager.LoadScene(newScene.ToString());
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
}
