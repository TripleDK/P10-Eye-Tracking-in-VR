using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FemaleButton : VRButton
{

    public override void Action(Controller side, VRGrab controller)
    {
        base.Action(side, controller);
        CalibrationContext.singleton.ChooseGender(1);
    }
}
