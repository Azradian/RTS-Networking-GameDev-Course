using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    // List of bases the game keeps track of
    private List<UnitBase> bases = new List<UnitBase>();

    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerhandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerhandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerhandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerhandleBaseDespawned;
    }

    [Server]
    private void ServerhandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerhandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        // Game continues as long as there's more than one player
        if (bases.Count != 1)
            return;

        Debug.Log("Game Over!");
    }

    #endregion

    #region Client
    #endregion
}
