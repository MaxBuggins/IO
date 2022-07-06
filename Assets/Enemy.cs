using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Hurtable
{
    public GameObject corpse;

    public override void OnDeath()
    {
        Instantiate(corpse, transform.position, transform.rotation, null);
        Destroy(gameObject);
    }
}
