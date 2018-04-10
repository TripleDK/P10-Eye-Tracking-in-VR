using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLatencyTest : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isServer)
            {
                //Send message that button is down
            }
            else
            {
                //Timestamp now, then wait for message from client and timestamp then
            }
        }
    }
}
