using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : Hurtful
{
    [Header("Hurt Propertys")]
    public float startDmgFallOff = 12;
    public float minDmgMultiplyer = 0.25f;

    [Header("Projectile Propertys")]
    //[Tooltip("Will skip collision with hurtfuls ignored collider")]
    //public bool skipIgnoreCollision;
    public float destroyDelay = 3;
    public float projectileWidth = 0.3f;

    public LayerMask serverMask; //for checking hits (Real)
    public LayerMask clientMask; //for checking rigid pushes (Decoration)

    [Header("Move Propertys")]
    public float forwardSpeed = 5; //if less than 0 then its a instant raycast
    public float gravitY = -9;
    public float airRistance = 0.1f;

    [Header("Internals")]
    public float timeSinceStart = 0;
    protected Vector3 orginPos;
    protected float distanceTraveld;

    private Vector3 velocity;

    [SyncVar]
    protected float randomNumber;

    [Header("Projectile Refrences")]
    private NetworkTransform netTrans;

    [Tooltip("Server Object for everyone - Must be listed as spawnable in the NetManager")]
    public GameObject hitNetworkObject;
    [Tooltip("Decoration only for clients")]
    public GameObject hitDecal;


    // Server and Clients must run
    void Awake()
    {
        if (!isClientOnly)
            randomNumber = Random.value;

        netTrans = GetComponent<NetworkTransform>();

        lastPos = transform.position;
        orginPos = transform.position;

        Invoke(nameof(DestroySelf), destroyDelay);
    }

    protected void Update()
    {
        timeSinceStart += Time.deltaTime;

        velocity.y = gravitY * timeSinceStart;
        Vector3 travel = ((Vector3.up * velocity.y) + (transform.forward * forwardSpeed)) * timeSinceStart;

        transform.position = orginPos + travel;

        distanceTraveld = travel.magnitude;
        forwardSpeed -= airRistance * timeSinceStart;

        if (isServer) //Adian Smells of car fuel
            ServerCheckHit();
        else
            ClientCheckHit();

        base.Update();
    }

    [Server]
    void ServerCheckHit()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
            out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
            serverMask, QueryTriggerInteraction.Ignore))
        {
            Hurtable hurtable = hit.collider.gameObject.GetComponentInParent<Hurtable>();
            if (hurtable != null)
            {
                if (hurtable == ignor)
                    return;

                //if (hurtful.ignorOwner && hurtable == hurtful.owner)
                //return; //dont interact with the shooter

                float dist = Vector3.Distance(orginPos, transform.position);
                int dmg = damage;

                if (dist > startDmgFallOff) //Dmg roll off math
                {
                    dist -= startDmgFallOff;
                    dmg = (int)(-0.0005f * (Mathf.Pow(dist, 4)) + dmg); //math moment with aidan
                    if (dmg < damage * minDmgMultiplyer) //min of 25% at about the distance of fog
                        dmg = (int)(damage * minDmgMultiplyer);
                }

                HurtObject(hurtable, dmg, hurtType);
            }


            ServerHit(hit.point, hit.normal);
        }

        if (isServerOnly == false)
            ClientCheckHit();
    }

    [Client]
    void ClientCheckHit() //does some extra client only stuff (rigidbodys)
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
            out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
            clientMask, QueryTriggerInteraction.Ignore))
        {
            Rigidbody hitRb = hit.collider.gameObject.GetComponentInParent<Rigidbody>();
            Hurtable hurtable = hit.collider.gameObject.GetComponentInParent<Hurtable>();

            if (hurtable != null)
            {
                if (hurtable == ignor)
                    return;
            }
            else if (hitRb != null)
            {
                Vector3 vel = (transform.position - lastPos) / Time.deltaTime;
                hitRb.AddForceAtPosition(vel * collisionForce, hit.point, ForceMode.Impulse);
            }


            ClientHit(hit.point, hit.normal);
        }
    }

    // everyoneDestroys
    [Server]
    void ServerHit(Vector3 hitPos, Vector3 hitNormal)
    {
        if (hitNetworkObject != null)
        {
            GameObject obj = Instantiate(hitNetworkObject, hitPos, transform.rotation);

            NetworkServer.Spawn(hitNetworkObject);

            //Hurtful hurt = obj.GetComponentInChildren<Hurtful>();
            //if (hurt != null)
                //hurt.owner = hurtful.owner;
        }

        DestroySelf(); //does not give client opertunity to instergate hit object
    }

    [Client]
    void ClientHit(Vector3 hitPos, Vector3 hitNormal)
    {
        if (hitDecal != null)
            Instantiate(hitDecal, hitPos, Quaternion.LookRotation(hitNormal));

/*        if (hitObject != null)
            Instantiate(hitObject, hitPos, transform.rotation);*/

        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    [ClientRpc]
    void RpcSyncProjectile(Vector3 pos, Vector3 rot, bool hit)
    {
        transform.position = pos;
        transform.eulerAngles = rot;

        if (hit && hitDecal != null)
            Instantiate(hitDecal, lastPos, transform.rotation);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f);

        Gizmos.DrawSphere(transform.position, projectileWidth);
    }
}
