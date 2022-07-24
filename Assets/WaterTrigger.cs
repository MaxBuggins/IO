using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{

    public AudioClip[] waterSounds;


    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if(rb != null)
        {
            AudioSource.PlayClipAtPoint(waterSounds[Random.Range(0, waterSounds.Length)], other.transform.position, 1.2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            AudioSource.PlayClipAtPoint(waterSounds[Random.Range(0, waterSounds.Length)], other.transform.position, 0.7f);
        }
    }
}
