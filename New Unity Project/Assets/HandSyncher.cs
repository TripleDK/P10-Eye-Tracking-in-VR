using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandSyncher : NetworkBehaviour
{
    [SyncVar]
    float _grabValue;

    public float grabValue
    {
        get { return _grabValue; }
        set
        {
            _grabValue = value;
            animator.SetFloat("GrabValue", _grabValue);
        }
    }
    public int handIndex = 0; //0 = left, 1 = right !!
    [SerializeField] Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }
}
