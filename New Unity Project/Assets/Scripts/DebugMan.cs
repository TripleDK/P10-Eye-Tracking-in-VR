using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMan : MonoBehaviour
{

    [SerializeField] GameObject debugMirror1;
    [SerializeField] GameObject debugMirror2;
    [SerializeField] GameObject debugMonitor;

    // Use this for initialization
    void Start()
    {
        debugMirror1.SetActive(true);
        debugMirror2.SetActive(true);
        debugMonitor.SetActive(true);
    }

}
