using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : Hurtable
{
    [HideInInspector] static public Player localInstance;

    [Header("Player Stats")] //spelling is for the weak, suck it up

    [SyncVar(hook = nameof(OnNameChanged))]
    public string userName = "NoNameNed";

    [SyncVar(hook = nameof(OnColourChanged))]
    public Color32 primaryColour = Color.black;

    [SyncVar(hook = nameof(OnHatChanged))]
    public int hatIndex = -1; //-1 means no hat

    [SyncVar] public int kills = 0; //umm no idear what this could mean
    [SyncVar] public int killStreak = 0; //how many kills before you respawn
    [SyncVar] public int assists = 0; //if you were helpful in someones death

    [SyncVar] public int deaths = 0; //you die you death

    [SyncVar(hook = nameof(UpdateBestTime))] 
    public float bestTime = -1;

    [SyncVar(hook = nameof(OnScoreChange))]
    public int bonusScore = 0; //For gameMode unique scores like capturing a flag

    public float timeSinceGrounded = 0;

    [HideInInspector] public MasterCheckPoint ownedRace;

    [HideInInspector] public MasterCheckPoint currentRace;
    public readonly SyncList<double> checkPointTimes = new SyncList<double>();

    [SyncVar(hook = nameof(OnCrouch))]
    public bool crouching = false;

    #region localStats
    //so things like status effects can be applyed without editing the
    //default values.
    [Header("Local Stats")]
    public CharacteristicsObject characteristicsObject;

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
    [HideInInspector] public PlayerController playerMovement;
    //private DirectionalSprite directionalSprite;
    private SkinnedMeshRenderer playerMeshRenderer;

    [SerializeField] private Transform headTransform;
    private GameObject hatObject;


    private void Awake()
    {
        netTrans = GetComponent<NetworkTransform>();
        character = GetComponentInChildren<CapsuleCollider>();
        playerMovement = GetComponent<PlayerController>();
        //directionalSprite = GetComponentInChildren<DirectionalSprite>();
        playerMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        levelManager = FindObjectOfType<LevelManager>();
        networkManager = FindObjectOfType<NetworkManager>();


        OnAlive(); //player is alived
    }

    private void Start()
    {
        LevelManager.instance.RefreshPlayerList();
    }

    public override void OnStartServer() 
    {

        SpawnPlayer();
    }

    public override void OnStartClient()
    {
        OnCrouch(crouching, crouching);
        base.OnStartClient();
    }

    public override void OnStartLocalPlayer()
    {
        localInstance = this;
        playerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        //directionalSprite.render.enabled = false;

        //playerMovement.enabled = true;
        playerCamera = Instantiate(playerCamera.gameObject, transform).GetComponent<PlayerCamera>();

        //UI_Main.instance.UIUpdate();


        CmdPlayerSetStats(LocalPlayerSettingsStorage.localInstance.localPlayerSettings.userName, LocalPlayerSettingsStorage.localInstance.localPlayerSettings.primaryColour);

        //set up SyncList
        checkPointTimes.Callback += OnTimesUpdated;

        base.OnStartLocalPlayer();
    }

    void UpdateBestTime(float oldBestTime, float newBestTime)
    {
        UI_Main.instance.UIUpdate();
    }

    [Command]
    public void CmdPlayerSetStats(string inUserName, Color32 inPrimaryColour)
    {
        if (inUserName.Contains("_7_")) //SECRET SHhhhhhhh
        {
            inUserName = inUserName.Remove(inUserName.IndexOf("_7_"), "_7_".Length);   
            hatIndex = 0;
        }

        userName = inUserName;

        primaryColour = inPrimaryColour;
    }

    [Command]
    public void CmdCrouch()
    {
        crouching = !crouching;
    }

    public void OnCrouch(bool _Old, bool _New)
    {
        if(_New == true)
        {
            character.height = crouchHeight;
        }
        else
        {
            character.height = standHeight;
        }

        character.center = Vector3.up * character.height / 2;

        if (isLocalPlayer)
        {
            playerCamera.Crouch(_New);
        }
    }
    
    public void OnScoreChange(int oldScore, int newScore)
    {
        if (newScore > oldScore)
        {
            if (isLocalPlayer)
            {
                UI_Main.instance.UIUpdate();
                UI_Main.instance.TemparyChangeScreenColour(new Color(0.572f, 0.863f, 0.729f, 0.3f), 0.5f);
            }
        }
    }
    public void OnNameChanged(string oldName, string newName)
    {
        UI_Main.instance.UIUpdate();
    }

    public void OnColourChanged(Color32 oldColor, Color32 newColor)
    {
        UI_Main.instance.UIUpdate();
    }

    public void OnHatChanged(int oldHat, int newHat)
    {
        if (hatObject != null)
            Destroy(hatObject);

        hatObject = Instantiate(MyNetworkManager.singleton.playerHats[newHat], headTransform);

        if (isLocalPlayer)
            hatObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    [ClientCallback]
    public override void OnHurt(int damage)
    {
        if (isLocalPlayer)
        {
            UI_Main.instance.UIUpdate();
            playerCamera.Shake(damage);

            if (health > 0)
                UI_Main.instance.TemparyChangeScreenColour(new Color(0.706f, 0.125f, 0.165f, 0.3f), 0.5f);
        }
    }


    public override void OnDeath()
    {
        playerMeshRenderer.gameObject.SetActive(false);
        //directionalSprite.render.enabled = false;
        character.enabled = false;

        Instantiate(corpse, transform.position, transform.rotation, null);

        if (isLocalPlayer)
        {
            playerCamera.Dead(true);
            UI_Main.instance.UIUpdate();
        }
    }


    public override void OnAlive()
    {
        character.enabled = true;

        if(currentRace != null)
            currentRace.EndRace(this, false);
        //currentCheckPoint = -1;

        playerMeshRenderer.gameObject.SetActive(true);

        if (isLocalPlayer)
        {
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerMovement.velocity = Vector3.zero;
            playerCamera.Dead(false);
            UI_Main.instance.UIUpdate();
        }

    }

    [ServerCallback]
    public override void ServerDeath()
    {
        character.enabled = false;

        if (ownedRace != null)
            if (ownedRace.finished == false)
                ownedRace.finished = true;

        deaths += 1;
        bonusScore = 0;
        Invoke(nameof(SpawnPlayer), levelManager.respawnDelay);
    }

    [ServerCallback]
    public void SpawnPlayer()
    {
        character.enabled = true;

        if (currentRace != null)
            currentRace.EndRace(this, false);

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

    void OnTimesUpdated(SyncList<double>.Operation operation, int index, double oldTime, double newTime)
    {
        switch (operation)
        {
            case SyncList<double>.Operation.OP_ADD:
                {
                    //currentCheckPoint++;
                    Color textColor = currentRace.colour;
                    textColor.a = 0.65f;

                    string msg = "";

                    if (index == 0)
                    {
                        UI_Main.instance.CreateAlert("|Start Race|", 60, textColor, alertObjIndex: 0);
                        return;
                    }

                    float time;
                    float roundedTime;
                    float duration = 2;

                    if (currentRace.checkPoints[checkPointTimes.Count - 1].finish)
                    {
                        msg = "Finish | ";
                        duration = 3;
                        time = (float)(newTime - checkPointTimes[0]);

                        currentRace.EndRace(this, true);
                    }

                    else
                    {
                        time = (float)(newTime - checkPointTimes[checkPointTimes.Count - 2]);
                    }
                    roundedTime = Mathf.Round(time * 1000.0f) / 1000.0f;
                    UI_Main.instance.CreateAlert(msg + roundedTime, 60, textColor, duration, alertObjIndex: 1);

                    break;
                }
        }
    }

    public void ClearRace()
    {
        currentRace = null;
        checkPointTimes.Clear();
        bestTime = -1;
    }
}