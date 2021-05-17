using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void HostLobby()
    {
        NetworkManager.singleton.StartHost();
    }
}
