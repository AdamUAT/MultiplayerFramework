using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    private Slider healthBar;
    [SerializeField]
    private GameObject healthBarPrefab;
    protected override void Die()
    {
        //Only trigger if this is the local player.
        if(Parent.IsLocalPlayer)
        {
            GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Dead);
        }

    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        healthBar.value = CurrentHealth / maxHealth;
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);

        healthBar.value = CurrentHealth / maxHealth;
    }

    /// <summary>
    /// Creates an instance of the HealthBar ui and attaches it to the controller.
    /// </summary>
    [ClientRpc]
    public void CreateHealthUIClientRpc()
    {
        //Only run if it is for the client's player.
        if(Parent.IsLocalPlayer)
        {
            GameObject healthBarCanvas = Instantiate(healthBarPrefab);
            healthBar = healthBarCanvas.GetComponentInChildren<Slider>();
        }
    }
}
