using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HurtTrigger : Hurtful
{
    private void OnTriggerEnter(Collider other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            HurtObject(hurtable, damage, hurtType);
        }
    }
}
