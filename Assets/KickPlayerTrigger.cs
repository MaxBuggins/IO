using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KickPlayerTrigger : MonoBehaviour
{

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        return;

        NetworkIdentity netIdentity = other.GetComponent<NetworkIdentity>();

        if(netIdentity != null)
        {
            if (!netIdentity.isServer)
                netIdentity.connectionToClient.Disconnect();
        }
    
    }
}
