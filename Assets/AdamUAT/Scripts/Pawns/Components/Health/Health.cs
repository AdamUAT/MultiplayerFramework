using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    private Controller parent;
    /// <summary>
    /// The controller this health component is attached to. 
    /// NOTE: unlike the other components, this parent is a controller, not pawn!
    /// </summary>
    public Controller Parent
    {
        get
        {
            return parent;
        }
        set
        {
            parent = value;
        }
    }

    [Tooltip("The maximum health this player can have.")]
    public float maxHealth { get; protected set; } = 100.0f;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    public float CurrentHealth
    {
        get
        {
            return currentHealth.Value;
        }
        protected set
        {
            SetHealthServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetHealthServerRpc(float value)
    {
        currentHealth.Value = value;
    }

    protected void Start()
    {
        CurrentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        CurrentHealth += amount;
        if(CurrentHealth > maxHealth)
        {
            //Prevents over-healing.
            CurrentHealth = maxHealth;
        }    
    }

    protected virtual void Die()
    {

    }
}
