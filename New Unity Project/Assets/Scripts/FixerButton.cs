using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FixerButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(1);
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
        StartCoroutine(WaitForAuthority());
    }

    IEnumerator WaitForAuthority()
    {
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdDestroyButton();
    }

    [Command]
    public void CmdDestroyButton()
    {
        NetworkServer.Destroy(gameObject);
    }
}
