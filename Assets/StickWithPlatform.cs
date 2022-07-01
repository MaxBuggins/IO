using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickWithPlatform : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 lastPos;

    private Player playerInTrigger;

    void FixedUpdate()
    {
        velocity = transform.position - lastPos;

        if (playerInTrigger != null)
        {
            if (playerInTrigger.health <= 0)
            {
                playerInTrigger = null;
                return;
            }

            //I was so wack i better ill still be wack now in the future
            //Vector3 relativeVelocity = (velocity / Time.fixedDeltaTime * 0.09); //not sure why 0.09

            //playerInTrigger.transform.position += velocity;
            playerInTrigger.playerMovement.rb.MovePosition(playerInTrigger.transform.position + velocity);
        }

        lastPos = transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (rb.tag == "Player")
            {
                Player player = rb.GetComponent<Player>();
                if (player == Player.localInstance)
                    playerInTrigger = player;

                return;
            }

            if (rb.isKinematic)
            {
                return;
            }

            rb.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (rb.tag == "Player")
            {
                Player player = rb.GetComponent<Player>();
                if (player == Player.localInstance)
                    playerInTrigger = null;

                return;
            }

            if (rb.isKinematic)
            {
                return;
            }

            rb.transform.parent = null;
        }
    }
}

