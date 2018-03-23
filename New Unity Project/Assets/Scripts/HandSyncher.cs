using System.Collections;
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
        Debug.Log("Grab value was changed by syncvar! " + _grabValue + ", isLocalPlayer: " + isLocalPlayer, animatorL.gameObject);
        grabValueL = _grabValue;
        if (!isLocalPlayer) animatorL.SetFloat("GrabValue", _grabValue);
    }

    void OnChangeGrabValueR(float _grabValue)
    {
        Debug.Log("Grab value was changed by syncvar! " + _grabValue + ", isLocalPlayer: " + isLocalPlayer, animatorR.gameObject);
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
        Debug.Log("Change value locally for: " + animatorL.gameObject.name + ", " + value);
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
        Debug.Log("Change value locally for: " + animatorR.gameObject.name + ", " + value);
        animatorR.SetFloat("GrabValue", value);
        CmdChangeGrabR(value);
    }

}
