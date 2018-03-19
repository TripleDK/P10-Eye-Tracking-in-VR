using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FixerButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(1);
        CmdDestroyButton();
    }

    [Command]
    public void CmdDestroyButton()
    {
        Destroy(gameObject);
    }
}
