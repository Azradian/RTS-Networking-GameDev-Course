using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;

    // Do this whenever a player is added to the server
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // Spawn in base for the player
        GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        // Give ownership to the respective base
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

}
