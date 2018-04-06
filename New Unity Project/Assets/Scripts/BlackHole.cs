using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlackHole : NetworkBehaviour
{

    public void TakeAuthority()
    {
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ObjectInteractions>())
        {
            Destroy(col.gameObject);
            NetworkIdentity playerId = col.gameObject.GetComponent<ObjectInteractions>().playerId;
            if (playerId == null)
                Debug.Log("That's weird!");
            else
                playerId.GetComponent<Player>().CmdSetAuth(TaskContext.singleton.netId, playerId);
            StartCoroutine(WaitForAuthorTaskContext());
        }
    }

    IEnumerator WaitForAuthorTaskContext()
    {
        while (!TaskContext.singleton.hasAuthority)
        {
            yield return null;
        }
        TaskContext.singleton.CmdNextObject();
    }


}
