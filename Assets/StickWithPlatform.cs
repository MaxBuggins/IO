using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickWithPlatform : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 lastPos;

    private Player playerInTrigger;
    private List<Rigidbody> rigidbodiesInTrigger = new List<Rigidbody>();

    void LateUpdate()
    {
        return;

        velocity = transform.position - lastPos;

        foreach(Rigidbody rb in rigidbodiesInTrigger)
        {
            rb.transform.Translate(velocity);
        }

        lastPos = transform.position;

        return;

        if (playerInTrigger != null)
        {
            print("InPlatform");
            if (playerInTrigger.health <= 0)
            {
                playerInTrigger = null;
                return;
            }

            //I was so wack i better ill still be wack now in the future
            //Vector3 relativeVelocity = (velocity / Time.fixedDeltaTime * 0.09); //not sure why 0.09

            playerInTrigger.playerMovement.MoveWithPlatform(velocity);
            //playerInTrigger.playerMovement.rb.MovePosition(playerInTrigger.transform.position + velocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            //rb.transform.parent = transform;
            if(rigidbodiesInTrigger.Contains(rb) == false)
                rigidbodiesInTrigger.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            //rb.transform.parent = transform;

            if (rigidbodiesInTrigger.Contains(rb))
                rigidbodiesInTrigger.Remove(rb);
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (rb.tag == "Player")
            {
                Player player = rb.GetComponent<Player>();
                if (player == Player.localInstance)
                {
                    playerInTrigger = player;
                    //player.transform.parent = transform;
                }

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
                {
                    playerInTrigger = null;
                    //player.transform.parent = null;
                }

                return;
            }

            if (rb.isKinematic)
            {
                return;
            }

            rb.transform.parent = null;
        }
    }*/
}

