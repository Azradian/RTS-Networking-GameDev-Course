using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;
    #region Server

    // Let the server create everyone's health
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageDealt)
    {
        if (currentHealth == 0)
            return;

        // This protects us from falling below 0 health and returns whichever is larger
        currentHealth = Mathf.Max(currentHealth - damageDealt, 0);

        if (currentHealth != 0)
            return;

        // Listens in 
        ServerOnDie?.Invoke();

        Debug.Log("We died");
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
