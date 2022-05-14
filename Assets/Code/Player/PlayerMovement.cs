using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float gravitY = -9.8f;


    [Header("Movement Internals")]
    [HideInInspector] public Vector2 move;

    public Vector3 velocity = Vector3.zero;

    private float timeSinceGrounded;
    private PhysicMaterial floorPhysicMat;

    private Vector3 floorNormal = Vector3.up;


    private Player player;
    private CharacterController character;
    private Camera cam;

    [HideInInspector] public Controls controls;

    public override void OnStartLocalPlayer() //just for the local client
    {

        controls = new Controls();

        controls.Play.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Play.Move.canceled += ctx => move = Vector2.zero;

        controls.Play.Crouch.performed += funny => Crouch();


        controls.Enable();

        enabled = true;
    }

    public override void OnStartServer() 
    { 

    }

    void Start()
    {
        player = GetComponent<Player>();
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
        ApplyMovement();
    }


    void ApplyMovement()
    {
        float velocitY = velocity.y; //save the fall (GUYS!!!!!!!) 
        velocity.y = 0; //dont use fall in calculations (GUYYYYYSSSS)

        move = move.normalized;
        float x = move.x;
        float z = move.y;


        Vector3 movement = transform.right * x + transform.forward * z;

        if (player.crouching)
            movement *= player.crouchSpeed;
        else
            movement *= player.speed;



        float fricktion = player.airFriction;

        if (character.isGrounded)
        {
            if (floorPhysicMat == null)
                fricktion = 0.3f;
            else
                fricktion = floorPhysicMat.dynamicFriction;
        }

        velocity.x = Mathf.Lerp(velocity.x, 0, fricktion);
        velocity.z = Mathf.Lerp(velocity.z, 0, fricktion);

        if (player.crouching)
            movement = Vector3.ClampMagnitude(movement + velocity, player.maxCrouchSpeed);
        else
            movement = Vector3.ClampMagnitude(movement + velocity, player.maxSpeed);

        movement.y = velocitY; //return velocity.y to unaffacted value

        character.Move(movement * Time.deltaTime); //apply movement to charhcter contoler

        velocity = movement;
    }

    [ClientCallback]
    void ApplyGravity()
    {
        if (character.isGrounded)
        {
            if (timeSinceGrounded > 0.4f)
                timeSinceGrounded = 0.4f;

            timeSinceGrounded -= Time.deltaTime * 4;

            if (velocity.y < 0) //if falling
                velocity.y = -1; //if grounded then no need to fall

            //if (jumpDelay > 0)
            //jumpDelay -= Time.fixedDeltaTime * 3;
        }
        else
        {
            if (timeSinceGrounded < 0 )
                timeSinceGrounded = 0;

            timeSinceGrounded += Time.deltaTime; //-= Time.deltaTime * 3; cant spam jump

            velocity.y += gravitY * Time.deltaTime; //two deltra time cause PHJYSCIS


            //if (jumpDelay < coyotTime)
            //jumpDelay += Time.fixedDeltaTime;

            //transform.parent = null;
            //transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }

    public void Crouch()
    {
        player.CmdCrouch();
        player.playerCamera.Crouch();
    }


    [ClientCallback]
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        floorPhysicMat = hit.collider.material;

        floorNormal = hit.normal;
        

        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 pushDir;

        //only for useable rigidbodys
        if (body == null)
            return;

        //dont push objects below
        if (hit.moveDirection.y < -0.3)
            return;
        else
        {
            pushDir = (hit.moveDirection);
        }

        // Apply the push
        body.velocity = pushDir * 5;
    }
}
