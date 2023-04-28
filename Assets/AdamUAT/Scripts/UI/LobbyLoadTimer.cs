using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class LobbyLoadTimer : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.UpdateTimer += LobbyLoadTimer_UpdateTimer;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the UI to match tell the player how long until the match starts. Time less than 0 will make the timer dissapear.
    /// </summary>
    private void LobbyLoadTimer_UpdateTimer(int newTime)
    {
        TextMeshProUGUI timerText = GetComponentInChildren<TextMeshProUGUI>();
        if (timerText != null)
        {
            if (newTime >= 0)
            {
                gameObject.SetActive(true);
                timerText.text = newTime.ToString();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Could not find lobby timer's text element.");
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.multiplayerManager.UpdateTimer -= LobbyLoadTimer_UpdateTimer;
    }
}
