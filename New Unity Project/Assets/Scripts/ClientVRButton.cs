using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientVRButton : VRButton
{

    NetworkManager networkManager;

    void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public override void Action()
    {
        networkManager.StartClient();
    }
}
