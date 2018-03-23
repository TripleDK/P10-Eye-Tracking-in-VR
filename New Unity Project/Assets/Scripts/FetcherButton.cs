using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FetcherButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(0);
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
        CmdDestroyButton();
    }

    [Command]
    public void CmdDestroyButton()
    {
        NetworkServer.Destroy(gameObject);
    }
}
