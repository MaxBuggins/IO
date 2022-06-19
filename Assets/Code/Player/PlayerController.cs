using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    private bool pause = false;

    [SerializeField] private float groundAcceleration = 100f;
    [SerializeField] private float airAcceleration = 100f;
    [SerializeField] private float groundLimit = 12f;
    [SerializeField] private float airLimit = 1f;

    [Header("Physcisc")]
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

    [HideInInspector] public Vector2 moveInput;
    private Vector3 moveRelative;

    private Vector3 groundNormal;
    private PhysicMaterial groundMaterial;

    private bool onGround = false;
    private bool jumpPending = false;
    private bool ableToJump = true;

    public Rigidbody rb;
    private PlayerWeapon playerWeapon;
    private Controls controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    public override void OnStartLocalPlayer() //just for the local client
    {
        controls = new Controls();

        controls.Menu.Pause.performed += funnyer => Pause();

        controls.Play.Move.performed += wasfunny => moveInput = wasfunny.ReadValue<Vector2>();
        controls.Play.Move.canceled += wasfunny => moveInput = Vector2.zero;

        controls.Play.Jump.performed += funny => jumpPending = true;
        controls.Play.Jump.canceled += funny => jumpPending = false;

        controls.Play.Die.performed += FunnyDeath => Suicide();

        //controls.Play.Crouch.performed += funnyer => Crouch();

        controls.Enable();

        enabled = true;
    }



    private void Update() 
    {
        GetMovementInput();
    }


    private void FixedUpdate() {
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
            onGround = false;

        if (onGround) {
            // Rotate movement vector to match ground tangent
            moveRelative = Vector3.Cross(Vector3.Cross(groundNormal, moveRelative), groundNormal);

            GroundAccelerate();
            ApplyGroundFriction();
        }
        else {
            ApplyGravity();
            AirAccelerate();
        }

        rb.velocity = velocity;

        // Reset onGround before next collision checks
        onGround = false;
        groundNormal = Vector3.zero;
    }

    void GetMovementInput() {
        float x = moveInput.x;
        float z = moveInput.y;

        //normalize the movement
        moveRelative = transform.rotation * new Vector3(x, 0f, z).normalized;
    }

    private void GroundAccelerate() {
        float addSpeed = groundLimit - Vector3.Dot(velocity, moveRelative);

        if (addSpeed <= 0)
            return;

        float accelSpeed = groundAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity += accelSpeed * moveRelative;

        if (clampGroundSpeed) {
            if (velocity.magnitude > groundLimit)
                velocity = velocity.normalized * groundLimit;
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

    private void ApplyGravity() {
        velocity.y -= gravitY * Time.deltaTime;
    }

    private void OnCollisionStay(Collision other) {

        groundMaterial = other.collider.material;

        // Check if any of the contacts has acceptable floor angle
        foreach (ContactPoint contact in other.contacts) {
            if (contact.normal.y > Mathf.Sin(slopeLimit * (Mathf.PI / 180f) + Mathf.PI/2f)) {
                groundNormal = contact.normal;
                onGround = true;
                return;
            }
        }
    }

    // This is for avoiding jumping all the time, thanks to the guy i stole this off
    private IEnumerator JumpTimer() {
        ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        ableToJump = true;
    }

    public void Pause()
    {
        pause = !pause;

        UI_Main.instance.OnPause(pause);

        //I should really centerlise the controls instead of a seperate Controls for each script
        if (pause) 
        {
            controls.Play.Disable();
            playerWeapon.controls.Disable();
        }
        else
        {
            controls.Play.Enable();
            playerWeapon.controls.Enable();
        }
    }

    public void Suicide() //13 11 14
    {
        Player.localInstance.CmdSelfHarm(Player.localInstance.health);
    }
}