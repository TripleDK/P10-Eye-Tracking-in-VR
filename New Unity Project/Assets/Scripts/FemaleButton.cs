using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FemaleButton : VRButton
{

    public override void Action()
    {
        CalibrationContext.singleton.ChooseGender(1);
    }
}
