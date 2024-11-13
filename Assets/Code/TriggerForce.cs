using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TriggerForce : NetworkBehaviour
{
    public Vector3 force;

    public bool localPlayerInTrigger = false;

    //public List<Player> playersInTrigger = new List<Player>();

    private void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (localPlayerInTrigger)
        {
            if (Player.localInstance.health <= 0) //They are dead kick them out
            {
                localPlayerInTrigger = false;
                return;
            }
            else
            {
                Vector3 addForce = force;

                //so if you fall
                if (Player.localInstance.playerMovement.velocity.y < 0)
                    addForce.y += -Player.localInstance.playerMovement.velocity.y / 2;

                Player.localInstance.playerMovement.rb.linearVelocity += addForce * Time.fixedDeltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if(player == Player.localInstance)
            {
                localPlayerInTrigger = true;
            }
            //playersInTrigger.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();


            if (player == Player.localInstance)
            {
                localPlayerInTrigger = false;
            }

            //playersInTrigger.Remove(player);
        }
    }
}
