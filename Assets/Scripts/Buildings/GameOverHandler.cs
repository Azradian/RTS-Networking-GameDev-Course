using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

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

        int playerID = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {playerID}");

        // Server enables game over
        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    // Server needs to tell the client the game is over, so use RPC
    [ClientRpc]
    private void RpcGameOver(string winnerName)
    {
        ClientOnGameOver?.Invoke(winnerName);
    }

    #endregion
}
