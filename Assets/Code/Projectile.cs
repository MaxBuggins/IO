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
    public int destoryOnHits = 0;
    public float destroyDelay = 3;
    public float projectileWidth = 0.3f;

    public LayerMask serverMask; //for checking hits (Real)
    public LayerMask clientMask; //for checking rigid pushes (Decoration)

    [Header("Move Propertys")]
    public float forwardSpeed = 5; //if less than 0 then its a instant raycast
    public float gravitY = -9;
    public float airRistance = 0.1f;

    [Header("Internals")]
    [HideInInspector] public float timeSinceStart = 0;
    private float currentForwardSpeed;
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
    public GameObject hitHurtableDecal;


    // Server and Clients must run
    void Awake()
    {
        if (!isClientOnly)
            randomNumber = Random.value;

        netTrans = GetComponent<NetworkTransform>();

        lastPos = transform.position;
        orginPos = transform.position;

        currentForwardSpeed = forwardSpeed;

        Invoke(nameof(DestroySelf), destroyDelay);
    }

    protected void Update()
    {
        timeSinceStart += Time.deltaTime;

        velocity.y = gravitY * timeSinceStart;
        Vector3 travel = ((Vector3.up * velocity.y) + (transform.forward * currentForwardSpeed)) * timeSinceStart;

        transform.position = orginPos + travel;

        distanceTraveld = travel.magnitude;
        currentForwardSpeed -= airRistance * timeSinceStart;
        print(travel);


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

            if (isServer)
                return;

            destoryOnHits -= 1;

            if (destoryOnHits < 0)
            {
                ClientHit(hit.point, hit.normal, hurtable);
            }

        }
    }

    // everyoneDestroys
    [Server]
    void ServerHit(Vector3 hitPos, Vector3 hitNormal)
    {
        if (destoryOnHits > -1)
        {
            destoryOnHits -= 1;

            if (destoryOnHits < 0)
            {
                DestroySelf(); //does not give client opertunity to instergate hit object
            }

            else
            {
                Vector3 forw = (transform.position - lastPos).normalized;
                Vector3 mirrored = Vector3.Reflect(forw, hitNormal);
                transform.rotation = Quaternion.LookRotation(mirrored, transform.up);
                transform.position = hitPos;

                velocity = Vector3.zero; //reset for gravity projectiles
                //currentForwardSpeed = forwardSpeed;
                timeSinceStart = 0;
                orginPos = transform.position;

                RpcSyncProjectile(transform.position, transform.eulerAngles, true);
            }
        }
    }

    [Client]
    void ClientHit(Vector3 hitPos, Vector3 hitNormal, Hurtable hurtable)
    {
        if (hurtable != null && hitHurtableDecal != null)
        {
            Instantiate(hitHurtableDecal, hitPos, Quaternion.LookRotation(hitNormal));
        }

        else if (hitDecal != null)
            Instantiate(hitDecal, hitPos, Quaternion.LookRotation(hitNormal));

        if(isClientOnly)
            Destroy(gameObject);
    }

    void DestroySelf()
    {
        if (hitNetworkObject != null && isServer)
        {
            GameObject obj = Instantiate(hitNetworkObject, transform.position, transform.rotation);

            NetworkServer.Spawn(obj);
        }

        Destroy(gameObject);
    }

    [ClientRpc]
    void RpcSyncProjectile(Vector3 pos, Vector3 rot, bool hit)
    {
        transform.position = pos;
        transform.eulerAngles = rot;

        velocity = Vector3.zero; //reset for gravity projectiles
                                 //currentForwardSpeed = forwardSpeed;
        timeSinceStart = 0;
        orginPos = transform.position;

        if (hit && hitDecal != null)
            Instantiate(hitDecal, lastPos, transform.rotation);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f);

        Gizmos.DrawSphere(transform.position, projectileWidth);
    }
}
