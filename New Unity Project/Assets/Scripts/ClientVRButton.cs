using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientVRButton : VRButton
{

    public override void Action()
    {
        CalibrationContext.singleton.ChooseNetwork(1);
    }
}
