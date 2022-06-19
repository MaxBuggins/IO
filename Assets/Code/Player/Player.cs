using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : Hurtable
{
    [HideInInspector] static public Player localInstance;

    [Header("Player Stats")] //spelling is for the weak, suck it up

    //[SyncVar(hook = nameof(OnNameChanged))]
    public string username = "NoNameNed";
    //[SyncVar(hook = nameof(OnColorChanged))]
    public Color32 colour = Color.black;

    [SyncVar] public int kills = 0; //umm no idear what this could mean
    [SyncVar] public int killStreak = 0; //how many kills before you respawn
    [SyncVar] public int assists = 0; //if you were helpful in someones death

    [SyncVar] public int deaths = 0; //you die you death

    [SyncVar] public int bonusScore = 0; //For gameMode unique scores like capturing a flag

    [SyncVar(hook = nameof(OnCrouch))]
    public bool crouching = false;

    #region localStats
    //so things like status effects can be applyed without editing the
    //default values.
    [Header("Local Stats")]

    public float speed = 8;
    public float crouchSpeed = 4;
    public float maxSpeed = 10;
    public float maxCrouchSpeed = 6;

    public float jumpHeight = 4;

    public float airFriction = 0.01f;

    public float standHeight = 2f;
    public float crouchHeight = 0.8f;

    #endregion

    [Header("Unity Stuff")]
    public PlayerCamera playerCamera;
    public GameObject corpse;

    private LevelManager levelManager;
    private NetworkManager networkManager;

    private NetworkTransform netTrans;
    private CapsuleCollider character;
    public PlayerController playerMovement;
    private DirectionalSprite directionalSprite;

    private void Awake()
    {
        netTrans = GetComponent<NetworkTransform>();
        character = GetComponentInChildren<CapsuleCollider>();
        playerMovement = GetComponent<PlayerController>();
        directionalSprite = GetComponentInChildren<DirectionalSprite>();

        levelManager = FindObjectOfType<LevelManager>();
        networkManager = FindObjectOfType<NetworkManager>();


        OnAlive(); //player is alived
    }

    public override void OnStartServer() 
    {
        SpawnPlayer();
    }

    public override void OnStartLocalPlayer()
    {
        localInstance = this;
        directionalSprite.render.enabled = false;

        //playerMovement.enabled = true;
        playerCamera = Instantiate(playerCamera.gameObject, transform).GetComponent<PlayerCamera>();

        //UI_Main.instance.UIUpdate();

        base.OnStartLocalPlayer();
    }

    [Command]
    public void CmdCrouch()
    {
        crouching = !crouching;
    }

    public void OnCrouch(bool _Old, bool _New)
    {
        if(crouching == true)
        {
            character.height = crouchHeight;
        }
        else
        {
            character.height = standHeight;
        }

        character.center = Vector3.up * character.height / 2;
    }
    

    [ClientCallback]
    public override void OnHurt(int damage)
    {
        if (isLocalPlayer)
        {
            //UI_Main.instance.UIUpdate();
            //gameCam.Shake(damage);
        }
    }


    public override void OnDeath()
    {
        directionalSprite.render.enabled = false;
        character.enabled = false;

        Instantiate(corpse, transform.position, transform.rotation, null);
    }


    public override void OnAlive()
    {
        character.enabled = true;


        if (isLocalPlayer)
        {
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerMovement.velocity = Vector3.zero;
        }
        else
        {
            if(directionalSprite.render != null)
                directionalSprite.render.enabled = true;
        }
    }

    [ServerCallback]
    public override void ServerDeath()
    {

        deaths += 1;
        bonusScore = 0;
        Invoke(nameof(SpawnPlayer), levelManager.respawnDelay);
    }

    [ServerCallback]
    public void SpawnPlayer()
    {
        Transform sPoint = networkManager.GetStartPosition();
        transform.position = sPoint.position;
        netTrans.RpcTeleport(sPoint.position);


        health = maxHealth;
    }

    public int GetScore() //common conversion for main score
    {
        //return (kills * 3 + assists + deaths * -1 + bonusScore); THIS IS FOR SHOOTERS
        return (bonusScore);
    }

    [TargetRpc]
    public override void TargetAddVelocity(NetworkConnection target, Vector3 vel) //TEMP apply to local player only
    {
        playerMovement.rb.velocity += vel;
        //playerMovement.velocity = Vector3.ClampMagnitude(playerMovement.velocity, maxVelocity); //no more infinit death demension
    }

    [TargetRpc]
    public override void TargetSetVelocity(NetworkConnection target, Vector3 vel, bool ignorZero) //TEMP apply to local player only
    {
        //Code some bitches
        if (vel.x == 0)
            vel.x = playerMovement.rb.velocity.x;
        if (vel.y == 0)
            vel.y = playerMovement.rb.velocity.y;
        if (vel.z == 0)
            vel.z = playerMovement.rb.velocity.z;

        playerMovement.rb.velocity = vel;
        //playerMovement.velocity = Vector3.ClampMagnitude(playerMovement.velocity, maxVelocity); //no more infinit death demension
    }

    [Command]
    public void CmdSelfHarm(int damage)
    {
        if (damage < 0) //Stop Heal Hacks
            return;

        Hurt(damage, HurtType.Suicide);
    }
}