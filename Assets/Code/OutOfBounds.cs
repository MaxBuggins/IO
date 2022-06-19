using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OutOfBounds : NetworkBehaviour
{
    public bool destoryRigidbody = false;
    [Min(0.1f)]
    public float disableDelay = 2f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body != null && other.tag != "Player") //dont destory the player
        {
            if (destoryRigidbody)
                Destroy(body.gameObject);
            else
                StartCoroutine(DisableRigidbody(body, disableDelay));
        }
    }


    IEnumerator DisableRigidbody(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        //rb.GetComponent<Mirror.Experimental.NetworkRigidbody>().enabled = false;
        //  Destroy(rb.GetComponent<NetworkIdentity>());
    }
}
