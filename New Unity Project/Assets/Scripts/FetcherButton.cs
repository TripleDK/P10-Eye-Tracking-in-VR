using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FetcherButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(0);
        CmdDestroyButton();
    }

    [Command]
    public void CmdDestroyButton()
    {
        Network.Destroy(gameObject);
    }
}
