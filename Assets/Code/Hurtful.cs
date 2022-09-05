using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public enum HurtType { Death, Suicide, Fall }

public class Hurtful : NetworkBehaviour
{
    [Header("Hurt")]
    public int damage = 1;
    public HurtType hurtType = HurtType.Death;

    public bool destoryOnHurt = false;

    public AudioClip hitSound;

    [Header("Force")]
    public bool moveForce = true; //if false then force is caculated via distance from collider center
    public float collisionForce = 0;
    public float upwardsForce = 0;
    public float maxVelocity = -1;

    protected Vector3 lastPos;
    protected Vector3 lastPos2; //this is beyond

    public Hurtable ignor;

    private void Start()
    {
        lastPos = transform.position;
    }

    protected void Update()
    {
        lastPos2 = lastPos; //beyond templeOS level programming
        lastPos = transform.position;
    }

    [Server]
    public void HurtObject(Hurtable hurtable, int damage, HurtType type)
    {
        print(name + " does " + damage + " to " + hurtable.name);

        if (damage == 0)
            return;

        if (hurtable == ignor)
            return;

        if (collisionForce > 0)
        {
            Vector3 vel;
            if (moveForce) //its a fix i guess
                vel = (transform.position - lastPos2) / Time.deltaTime;
            else
                vel = (hurtable.transform.position - transform.position).normalized;

            if (maxVelocity > 0)
                vel = Vector3.ClampMagnitude(vel, maxVelocity);

            if(hurtable.connectionToClient != null)
                hurtable.TargetAddVelocity(hurtable.connectionToClient, (vel * collisionForce) + (vel.magnitude * Vector3.up * upwardsForce));
        }

        if (hitSound != null)
        {
            if (ignor == Player.localInstance)
                UnityExtensions.PlayAudioAt(hitSound, hurtable.transform.position, 1, 0);
            else
                UnityExtensions.PlayAudioAt(hitSound, hurtable.transform.position, 0.5f, 1);
        }


        hurtable.Hurt(damage, type);

        if (destoryOnHurt == true)
            Destroy(gameObject);
    }
}
