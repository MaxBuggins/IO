using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HurtTriggerAdvance : Hurtful
{
    public AnimationCurve hurtDistanceCurve;

    private Collider ownCollider;


    public void CheckSphere(float radius)
    {
        ownCollider = GetComponent<Collider>();

        foreach (Collider c in Physics.OverlapSphere(transform.position, radius))
        {
            var hurtable = c.GetComponent<Hurtable>();
            if (hurtable != null)
            {
                float relativeDistance = Vector3.Distance(c.transform.position, transform.position) / radius;
                HurtObject(hurtable, (int)(damage * Mathf.Abs(relativeDistance - 1)), hurtType);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            float relativeDistance = Vector3.Distance(other.transform.position, transform.position) / ownCollider.bounds.size.magnitude;
            HurtObject(hurtable, (int)(damage * Mathf.Abs(relativeDistance - 1)), hurtType);
        }
    }
}