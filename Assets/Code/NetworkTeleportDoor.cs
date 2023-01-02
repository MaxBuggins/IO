using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Renderer))] //fancy
public class NetworkTeleportDoor : NetworkBehaviour, Interact
{
    [SyncVar(hook = nameof(UpdateDoorState))] public bool open;

    [Header("Interaction")]
    public float maxInteractDistanceValue = 2;
    public float maxInteractDistance
    {
        get { return maxInteractDistanceValue; }
        set { maxInteractDistanceValue = value; }
    }

    [Header("Visual")]
    [SerializeField] private float openDuration = 1f;
    [HideInInspector] public float timeTillClose = 0;
    public Material openMaterial;
    public Material closeMaterial;

    [Header("Sound")]
    public AudioClip openSound;
    public AudioClip closeSound;


    [Header("Refrences")]
    public NetworkTeleportDoor desternationDoor;
    public Transform ownDesternationTrans;

    private Renderer render;
    private AudioSource audioSource;

    void Start()
    {
        render = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
    }

    
    private void Update()
    {
        if (isServer == false)
            return;

        if (open)
        {
            timeTillClose -= Time.deltaTime;

            if(timeTillClose < 0)
            {
                open = false;
            }
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdUseDoor(NetworkConnectionToClient sender = null)
    {
        //Server Sided Check
        if (Vector3.Distance(sender.identity.transform.position, transform.position) > maxInteractDistance) 
            return;

        open = true;
        timeTillClose = openDuration;

        desternationDoor.open = true;
        desternationDoor.timeTillClose = openDuration;


        sender.identity.GetComponent<NetworkTransformBase>().RpcTeleport(desternationDoor.ownDesternationTrans.position, desternationDoor.ownDesternationTrans.rotation);
    }

    [ClientCallback]
    public void Interact(float distance)
    {
        if (distance > maxInteractDistance)
            return;

        CmdUseDoor();
    }

    void UpdateDoorState(bool oldBool, bool newBool)
    {
        audioSource.pitch = Random.Range(0.7f, 1.4f);
        audioSource.volume = Random.Range(0.8f, 1f);

        if (newBool == true)
        {
            audioSource.PlayOneShot(openSound);
            render.material = openMaterial;
        }

        else
        {
            audioSource.PlayOneShot(closeSound);
            render.material = closeMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, desternationDoor.ownDesternationTrans.position);
    }
}
