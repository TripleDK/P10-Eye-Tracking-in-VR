using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticButton : VRButton
{
    public override void Action(Controller side, VRGrab controller)
    {
        base.Action(side, controller);
        CalibrationContext.singleton.ChooseStyle(0);
    }
}
