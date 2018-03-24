﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandSyncher : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeGrabValueL")]
    public float grabValueL;
    [SyncVar(hook = "OnChangeGrabValueR")]
    public float grabValueR;

    void OnChangeGrabValueL(float _grabValue)
    {
        grabValueL = _grabValue;
        if (!isLocalPlayer) animatorL.SetFloat("GrabValue", _grabValue);
    }

    void OnChangeGrabValueR(float _grabValue)
    {
        grabValueR = _grabValue;
        if (!isLocalPlayer) animatorR.SetFloat("GrabValue", _grabValue);
    }

    [SerializeField] Animator animatorL;
    [SerializeField] Animator animatorR;

    [Command]
    public void CmdChangeGrabL(float value)
    {
        grabValueL = value;
    }

    public void ChangeGrabL(float value)
    {
        animatorL.SetFloat("GrabValue", value);
        CmdChangeGrabL(value);
    }

    [Command]
    public void CmdChangeGrabR(float value)
    {
        grabValueR = value;
    }

    public void ChangeGrabR(float value)
    {
        animatorR.SetFloat("GrabValue", value);
        CmdChangeGrabR(value);
    }

}
