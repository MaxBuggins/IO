using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class Enemy : Hurtable
{
    [Header("Scale")]
    private Vector3 startScale;
    public Vector3 endScaleMax;
    public Vector3 endScaleMin;
    public float duration = 0.15f;
    public AnimationCurve hitAnimationCurve;

    public GameObject corpse;

    public override void OnHurt(int damage)
    {
        Vector3 endScale = new Vector3(Random.Range(endScaleMin.x, endScaleMax.x),
            Random.Range(endScaleMin.y, endScaleMax.y),
            Random.Range(endScaleMin.z, endScaleMax.z));

        Tween.LocalScale(transform, endScale, duration, 0, hitAnimationCurve);
    }

    public override void OnDeath()
    {
        if(corpse != null)
            Instantiate(corpse, transform.position, transform.rotation, null);


        if(isClientOnly)
            Destroy(gameObject);
    }

    public override void ServerDeath()
    {
        Invoke(nameof(DestorySelf), 0.3f); //give the client time to recive the death call
    }

    public void DestorySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
