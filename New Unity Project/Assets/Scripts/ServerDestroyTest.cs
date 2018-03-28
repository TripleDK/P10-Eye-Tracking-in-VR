using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerDestroyTest : NetworkBehaviour
{
    public KeyCode keyToDestroy;
    // Use this for initialization
    void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(keyToDestroy))
        {
            Debug.Log("Clicked!");
            NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
            playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
            StartCoroutine(WaitForAuthority());
        }
    }
    void OnMouseDown()
    {

    }

    IEnumerator WaitForAuthority()
    {
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdDestroy();
    }

    [Command]
    public void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

}
