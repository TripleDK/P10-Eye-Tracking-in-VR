using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixerButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(1);
    }
}
