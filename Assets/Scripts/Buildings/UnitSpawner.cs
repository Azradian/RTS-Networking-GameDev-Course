using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnTime = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
            ProduceUnits();

        if (isClient)
            UpdateTimerDisplay();
    }

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
        // Check if we are at max units being queued
        if (queuedUnits == maxUnitQueue)
            return;

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        // Check if the player has enough resources to queue the unit
        if (player.GetResources() < unitPrefab.GetResourceCost())
            return;

        queuedUnits++;

        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0)
            return;

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnTime)
            return;

        GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
       
        // The unit that's spawned belongs to the client
        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        // We have created our unit and moved it out of the way, time to clean up for the next one!
        queuedUnits--;
        unitTimer = 0;
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnTime;

        // After we complete a queue for a unit, restart. Otherwise cycle through the fill amount until it is complete
        if (newProgress < unitProgressImage.fillAmount)
            unitProgressImage.fillAmount = newProgress;
        else
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
    }

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

    private void ClientHandleQueuedUnitsUpdated(int oldValue, int newValue)
    {
        remainingUnitsText.text = newValue.ToString();
    }

    #endregion
}
