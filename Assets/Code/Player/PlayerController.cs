using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Steamworks;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : NetworkBehaviour
{
    private bool pause = false;

    [SerializeField] private float groundAcceleration = 100f;
    [SerializeField] private float airAcceleration = 100f;
    [SerializeField] private float groundLimit = 12f;
    [SerializeField] private float airLimit = 1f;
    [SerializeField] private float crouchSpeedMultiplyer = 0.5f;

    [Header("Physcis")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float gravitY = 16f;
    //[SerializeField] private float airFriction = 2f;
    [SerializeField] private float groundBaseFriction = 12f;
    [SerializeField] private float rampSlideLimit = 5f;
    [SerializeField] private float slopeLimit = 45f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private bool additiveJump = true;
    [SerializeField] private bool autoJump = true;

    [Header("Etc")]
    [SerializeField] private bool clampGroundSpeed = false;
    [SerializeField] private bool disableBunnyHopping = false;

    public Vector3 velocity;
    public Vector3 externalVelocity;

    [HideInInspector] public Vector2 moveInput;
    private Vector3 moveRelative;

    private Vector3 groundNormal;
    private PhysicMaterial groundMaterial;

    [HideInInspector] public bool onGround = false;
    private bool jumpPending = false;
    private bool ableToJump = true;

    [HideInInspector] public PlayerInput playerInput;
    public Rigidbody rb;
    private Player player;
    private Controls controls;


    [Header("Stats")]
    float distanceSurfed;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    public override void OnStartLocalPlayer() //just for the local client
    {
        controls = new Controls();

        controls.Play.Move.performed += wasfunny => moveInput = wasfunny.ReadValue<Vector2>();
        controls.Play.Move.canceled += wasfunny => moveInput = Vector2.zero;

        controls.Play.Jump.performed += funnyer => jumpPending = true;
        controls.Play.Jump.canceled += funnyer => jumpPending = false;

        controls.Play.Callout.performed += moreFUNNY => player.CmdPlayAudioClip((int)moreFUNNY.ReadValue<float>());

        controls.Menu.Pause.performed += funner => OnPause();

        controls.Enable();

        enabled = true;
        playerInput.enabled = true;

        SteamUserStats.GetStat("distance_surfed", out distanceSurfed);
    }


    private void Update() 
    {
        GetMovementInput();

    }


    private void FixedUpdate() 
    {
        if (!onGround)
            player.timeSinceGrounded += Time.fixedDeltaTime;
        else
            player.timeSinceGrounded -= Time.fixedDeltaTime;


        if (transform.position.y < LevelManager.instance.minSightHeight)
            UI_Main.instance.ChangeScreenColour(new Color(0.8f, 0.8f, 0.9f, 1));


        velocity = rb.velocity; 

        // Clamp speed for bunny hop more like funny hop
        if (disableBunnyHopping && onGround) {
            if (velocity.magnitude > groundLimit)
                velocity = velocity.normalized * groundLimit;
        }

        // Jump wow
        if (jumpPending && onGround) {
            Jump();
        }

        // Air physics if moving upwards fast (Maybe Stupid)
        if (rampSlideLimit >= 0f && velocity.y > rampSlideLimit)
        {
            onGround = false;
        }

        if (onGround)
        {
            // Rotate movement vector to match ground tangent
            moveRelative = Vector3.Cross(Vector3.Cross(groundNormal, moveRelative), groundNormal);

            GroundAccelerate();
            ApplyGroundFriction();
        }
        else
        {
            ApplyGravity();
            AirAccelerate();
        }

        rb.velocity = velocity + externalVelocity;
        externalVelocity = Vector3.zero;

        if (onGround == false)
        {
            distanceSurfed += (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z)) * Time.fixedDeltaTime;
            SteamUserStats.SetStat("distance_surfed", distanceSurfed);
        }

        // Reset onGround before next collision checks
        onGround = false;
        groundNormal = Vector3.zero;
    }


    void GetMovementInput() {
        float x = Mathf.Round(moveInput.x * 2) / 2;
        float z = Mathf.Round(moveInput.y * 2) / 2;

        //normalize the movement
        moveRelative = transform.rotation * new Vector3(x, 0f, z).normalized;
    }

    private void GroundAccelerate() {

        float addSpeed = groundLimit - Vector3.Dot(velocity, moveRelative);

        if (addSpeed <= 0)
            return;

        float accelSpeed = groundAcceleration * Time.deltaTime;

        if (player.crouching)
            accelSpeed *= crouchSpeedMultiplyer;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity += accelSpeed * moveRelative;

        if (clampGroundSpeed) 
        {
            float _groundLimit = groundLimit;
            if (player.crouching)
                _groundLimit *= crouchSpeedMultiplyer;

            if (velocity.magnitude > _groundLimit)
                velocity = velocity.normalized * _groundLimit;
        }
    }

    //Valve be like, ok sure yeah lets use a dot
    private void AirAccelerate() {
        Vector3 hVel = velocity;
        hVel.y = 0;

        float addSpeed =  airLimit - Vector3.Dot(hVel, moveRelative);

        if (addSpeed <= 0)
            return;

        float accelSpeed = airAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity += accelSpeed * moveRelative;
    }

    private void ApplyGroundFriction() 
    {
        float fricktionMultiplyer = 0.5f;

        if (groundMaterial != null)
            fricktionMultiplyer = groundMaterial.dynamicFriction; //Use friction from floor material

       velocity *= Mathf.Clamp01(1 - Time.deltaTime * (groundBaseFriction * fricktionMultiplyer));

    }

    private void Jump() {
        if (!ableToJump)
            return;

        if (velocity.y < 0f || !additiveJump)
            velocity.y = 0f;

        velocity.y += jumpHeight;
        onGround = false;

        if (!autoJump)
            jumpPending = false;

        StartCoroutine(JumpTimer());
    }

    private void ApplyGravity() 
    {  
        velocity -= (transform.up * gravitY) * Time.deltaTime;
    }

    [ClientCallback]
    private void OnCollisionStay(Collision other) //On Collision Stay is applicable? (True)
    {
        if (UnityExtensions.Contains(groundLayerMask, other.gameObject.layer) == false)
            return;

        groundMaterial = other.collider.material;

        onGround = false;

        bool surfing = true;

        // Check if any of the contacts has acceptable floor angle
        foreach (ContactPoint contact in other.contacts)
        {
            print(contact.normal.y);

            if (contact.normal.y > Mathf.Sin(slopeLimit * (Mathf.PI / 180f) + Mathf.PI / 2f))
            {
                if (player.timeSinceGrounded > 1f)
                {
                    player.playerAnimator.OnHardLanding();
                }

                groundNormal = contact.normal;
                onGround = true;
                player.timeSinceGrounded = 0;
                player.CmdSetSurfing(false);
                surfing = false;
                return;
            }

            if (contact.normal.y < 0.05f)
            {
                surfing = false;
                player.CmdSetSurfing(false);
                return;
            }
        }
        if (surfing =! player.surfing)
            player.CmdSetSurfing(surfing);

    }

    // This is for avoiding jumping all the time, thanks to the guy i stole this off
    private IEnumerator JumpTimer() {
        ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        ableToJump = true;
    }

    public void OnPause()
    {
        pause = !pause;

        UI_Main.instance.OnPause(pause);


        //I should really centerlise the controls instead of a seperate Controls for each script
        if (pause) 
        {
            controls.Play.Disable();
            player.playerCamera.controls.Disable();
        }
        else
        {
            controls.Play.Enable();
            player.playerCamera.controls.Enable();
        }
    }

    public void OnCrouch()
    {
        Player.localInstance.CmdCrouch();
    }

    public void OnSuicide() //13 11 14
    {
        Player.localInstance.CmdSelfHarm(Player.localInstance.health);
    }

    public void OnShowScoreBoard()
    {
        UI_Main.instance.ShowScoreboard(!UI_Main.instance.scoreBoardUI.active);
    }

    public void MoveWithPlatform(Vector3 movement)
    {
        externalVelocity += movement;
        rb.velocity += movement;
    }
}