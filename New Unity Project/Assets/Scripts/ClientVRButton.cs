using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientVRButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseNetwork(1);
    }
}
