using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RB_StartForce : MonoBehaviour
{
    public Vector3 maxForce;
    public Vector3 minForce;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(Random.Range(minForce.x, maxForce.x),
            Random.Range(minForce.y, maxForce.y),
            Random.Range(minForce.z, maxForce.z), ForceMode.VelocityChange);
    }
}
