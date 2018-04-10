using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class BlackHole : NetworkBehaviour
{
    public UnityEvent OnEatObject = new UnityEvent();

    public void TakeAuthority()
    {
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ObjectInteractions>())
        {
            OnEatObject.Invoke();
            if (col.gameObject.name == "Sphere(Clone)")
            {
                return;
            }
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
