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
    protected Vector3 orginPos;
    protected float distanceTraveld;

    private Vector3 velocity;

    [SyncVar]
    protected float randomNumber;

    [Header("Projectile Refrences")]
    private NetworkTransform netTrans;

    public GameObject hitObject;
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
        velocity.y += gravitY * Time.deltaTime;
        Vector3 travel = ((Vector3.up * velocity.y) + (transform.forward * forwardSpeed)) * Time.deltaTime;

        transform.position = transform.position + travel;

        distanceTraveld += travel.magnitude;
        forwardSpeed -= airRistance * Time.deltaTime;

        if (isServer) //Adian Smells of car fuel
            CheckHit();

        else
            CheckPush();



        base.Update();
    }

    [Server]
    void CheckHit()
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

            ServerHit();
        }

        if (isServerOnly == false)
            CheckPush();
    }

    [Client]
    void CheckPush()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
            out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
            clientMask, QueryTriggerInteraction.Ignore))
        {
            Rigidbody hitRb = hit.collider.gameObject.GetComponentInParent<Rigidbody>();

            if (hitRb != null)
            {
                Vector3 vel = (transform.position - lastPos) / Time.deltaTime;
                hitRb.AddForceAtPosition(vel * collisionForce, hit.point, ForceMode.Impulse);
            }

            else
            {
                if (hitDecal != null)
                    Instantiate(hitDecal, hit.point, Quaternion.LookRotation(hit.normal));

                ClientHit();
            }
        }
    }

    // everyoneDestroys
    [Server]
    void ServerHit()
    {
        if (hitObject != null)
        {
            GameObject obj = Instantiate(hitObject, lastPos, transform.rotation);

            //Hurtful hurt = obj.GetComponentInChildren<Hurtful>();
            //if (hurt != null)
                //hurt.owner = hurtful.owner;
        }

        DestroySelf();
    }

    [Client]
    void ClientHit()
    {
        if (hitObject != null)
            Instantiate(hitObject, lastPos, transform.rotation);

        //if (hitSplat != null)
        //Instantiate(hitSplat, lastPos, transform.rotation);

        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    [ClientRpc]
    void RpcSyncProjectile(Vector3 pos, Vector3 rot, bool hit)
    {
        if (hit && hitObject != null)
            Instantiate(hitObject, lastPos, transform.rotation);

        transform.position = pos;
        transform.eulerAngles = rot;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f);

        Gizmos.DrawSphere(transform.position, projectileWidth);
    }
}
