using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class RbExsplode : MonoBehaviour
{
    public float exsplosionForce;
    public float exsplosionRange;

    public float shakeRange = 15;

    public float despawnDuration;

    Rigidbody[] rbs;


    public void Exsplode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, exsplosionRange);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(exsplosionForce, explosionPos, exsplosionRange, 0.4f);
        }

        foreach(Player player in LevelManager.instance.players)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < shakeRange)
            {
                player.playerCamera.Shake(Mathf.Abs((distance / -shakeRange) - 1) * 1.2f);
            }
        }
    }
}

