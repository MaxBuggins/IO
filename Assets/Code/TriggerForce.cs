using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TriggerForce : NetworkBehaviour
{
    public Vector3 force;

    public List<Player> playersInTrigger = new List<Player>();

    void Start()
    {
        if (isClientOnly)
            Destroy(this);
    }

    void FixedUpdate()
    {
        foreach (Player player in playersInTrigger)
        {
            Vector3 addForce = force;

            if (player.health <= 0) //They are dead kick them out
            {
                playersInTrigger.Remove(player);

            }
            else
            {
                //so if you fall
                if (player.playerMovement.velocity.y < 0)
                    addForce.y += -player.playerMovement.velocity.y / 2;

                player.TargetAddVelocity(player.connectionToClient, addForce * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            playersInTrigger.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            playersInTrigger.Remove(player);
        }
    }
}
