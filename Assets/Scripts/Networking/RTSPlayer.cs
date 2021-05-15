using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    private Color teamColor = new Color();

    private List<Unit> playerUnits = new List<Unit>();
    private List<Building> playerBuildings = new List<Building>();

    public event Action<int> ClientOnResourcesUpdated;

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public List<Unit> GetPlayerUnits()
    {
        return playerUnits;
    }

    public List<Building> GetPlayerBuildings()
    {
        return playerBuildings;
    }

    public int GetResources()
    {
        return resources;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        // Check if we are overlapping with another building
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            return false;

        foreach (Building building in playerBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
                return true;
        }

        return false;
    }

    #region Server
    public override void OnStartServer()
    {
        // We have a player! Allow for spawning and despawning
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        // Player has left, remove the ability to spawn and despawn
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 spawnpoint)
    {
        // Figure out which building is which ID
        Building buildingToPlace = null;

        // loop over each building to find the id we're looking for
        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        // Can't find the buildingId requested, lets leave
        if (buildingToPlace == null)
            return;

        // Can the player afford the building?
        if (resources < buildingToPlace.GetPrice())
            return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, spawnpoint))
            return;

        // Now we spawn the proper building and give authority to the player spawning it
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, spawnpoint, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        // Now we subtract the cost
        SetResources(resources - buildingToPlace.GetPrice());
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        playerUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        playerUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        playerBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        playerBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
            return;

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        // Whenever our value is updated, tell the UI the new resource value
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        playerUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        playerUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        playerBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        playerBuildings.Remove(building);
    }

    #endregion
}
