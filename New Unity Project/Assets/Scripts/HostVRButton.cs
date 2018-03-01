using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostVRButton : VRButton
{

    public override void Action()
    {
        CalibrationContext.singleton.ChooseNetwork(0);
    }
}
