using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestPlayer : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer) gameObject.tag = "LocalPlayer";
    }
}
