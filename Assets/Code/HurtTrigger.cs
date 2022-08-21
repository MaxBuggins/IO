using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HurtTrigger : Hurtful
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                //if (ignorOwner == false || player != owner)
                //{
                    HurtObject(player, damage, hurtType);
                //}
            }
            return;
        }
    }
}
