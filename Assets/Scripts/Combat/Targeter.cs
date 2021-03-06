using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Found on all units to set the current target

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    #region Server

    [Server]
    private void ServerHandleGameOver()
    {
        // When the game is over, clear our target
        ClearTarget();
    }

    // Set the target across the network
    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        // Check if the target is actually targettable
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target))
            return;

        this.target = target;
    }

    // Now we need a way to clear the current target
    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    #region Client

    #endregion

}
