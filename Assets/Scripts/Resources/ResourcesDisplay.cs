﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (player != null)
            {
                ClientHandleResourcesUpdated(player.GetResources());

                player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            }
        }
    }

    // This will trigger when a player is destroyed, aka leaves the game
    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
