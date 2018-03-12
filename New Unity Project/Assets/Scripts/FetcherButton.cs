using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetcherButton : VRButton
{

    public override void Action(Controller side)
    {
        CalibrationContext.singleton.ChooseRole(0);
        Network.Destroy(gameObject);
    }
}
