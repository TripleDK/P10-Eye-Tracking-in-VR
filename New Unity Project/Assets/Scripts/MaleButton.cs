using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MaleButton : VRButton
{
    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseGender(0);
    }
}
