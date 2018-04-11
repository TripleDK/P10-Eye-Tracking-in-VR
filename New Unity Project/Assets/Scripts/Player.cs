using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [Command]
    public void CmdSetAuth(NetworkInstanceId objectId, NetworkIdentity player)
    {
        var iObject = NetworkServer.FindLocalObject(objectId);
        if (iObject == null)
        {
            Debug.LogWarning("Object not found by server!");
            return;
        }
        var networkIdentity = iObject.GetComponent<NetworkIdentity>();
        var otherOwner = networkIdentity.clientAuthorityOwner;

        if (otherOwner == player.connectionToClient)
        {
            return;
        }
        else
        {
            if (otherOwner != null)
            {
                networkIdentity.RemoveClientAuthority(otherOwner);

            }
            networkIdentity.AssignClientAuthority(player.connectionToClient);
        }


    }
}
