using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPageUI = null;

    public void HostLobby()
    {
        landingPageUI.SetActive(false);

        NetworkManager.singleton.StartHost();
    }
}
