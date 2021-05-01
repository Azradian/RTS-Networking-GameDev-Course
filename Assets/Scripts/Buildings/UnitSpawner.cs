using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        // The unit that's spawned belongs to the client
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    // Whenever this object is clicked, do this
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the left mouse button is hit
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Lets make sure the only person who can interact is the owner
        if (!hasAuthority)
            return;

        CmdSpawnUnit();
    }

    #endregion
}
