using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public event Action ServerOnDie;

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

    #endregion
}
