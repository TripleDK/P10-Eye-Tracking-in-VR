using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandSyncher : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeGrabValue")]
    public float grabValue;

    void OnChangeGrabValue(float _grabValue)
    {
        animator.SetFloat("GrabValue", _grabValue);
    }
    public int handIndex = 0; //0 = left, 1 = right !!
    [SerializeField] Animator animator;


}
