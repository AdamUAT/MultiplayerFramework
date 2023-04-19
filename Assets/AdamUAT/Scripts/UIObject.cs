using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameState this UI object will be used in.")]
    private GameStateManager.GameState gameState;

    private void Awake()
    {
        GameManager.instance.uiManager.AddUIObject(this);
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
        if(gameState == associatedGameState)
        {
            gameObject.SetActive(false);
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
        if (gameState == associatedGameState)
        {
            gameObject.SetActive(true);
        }
    }
}
