using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : Hurtable
{
    public GameObject corpse;

    public override void OnDeath()
    {
        Instantiate(corpse, transform.position, transform.rotation, null);


        if(isClientOnly)
            Destroy(gameObject);
    }

    public override void ServerDeath()
    {
        Invoke(nameof(DestorySelf), 0.3f); //give the client time to recive the death call
    }

    public void DestorySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
