using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Collider))]
public class HostonlyArea : MonoBehaviour
{
    Collider collision;


    void Start()
    {
        collision = GetComponent<Collider>();

        if (NetworkServer.active)
            collision.enabled = false;
    }
}
