using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    [HideInInspector] static public PlayerCamera localInstance;

    [Header("Variables")]
    public bool lockYRot = false;
    private float xRotation = 0f;

    public float mouseLookSensitivty = 18;
    public float keyLookSensitivty = 50;
    public float maXLook = 10;
    public float rotToFloor = 8;

    [Header("Offsets")]
    [HideInInspector] public Vector3 cameraOffset;
    public Vector2 standCameraOffset;
    public Vector2 crouchCameraOffset;

    [Header("HeadBob")]
    public float bobSpeed = 1;
    public float bobAmount = 0.25f;
    private float bobTime = 0;

    [Header("Shake")]
    public float shakeDuration = 0.5f;
    public float magnatude = 0.3f;

    public float rotResetSpeed = 1;

    [Header("Internal Variables")]
    public Vector2 look;
    private Vector3 lastPos;
    [HideInInspector]public Vector3 currentOffset = Vector3.zero;
    [HideInInspector] public Transform focus;
    [SerializeField] private Collider deadCollider;

    private PlayerAboveInfo currentPlayerAboveInfo;


    [Header("Unity Things")]
    public PlayerController movement;
    private Controls controls;


    void Start()
    {
        localInstance = this;

        movement = GetComponentInParent<PlayerController>();

        controls = new Controls();

        controls.Play.MouseLook.performed += ctx => look = ctx.ReadValue<Vector2>() * mouseLookSensitivty;
        controls.Play.MouseLook.canceled += ctx => look = Vector2.zero;

        controls.Play.KeyLook.performed += ctx => look = ctx.ReadValue<Vector2>() * keyLookSensitivty;
        controls.Play.KeyLook.canceled += ctx => look = Vector2.zero;

        controls.Enable();
        Cursor.lockState = CursorLockMode.Locked;

        mouseLookSensitivty = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.mouseSensativity;
        SetThirdPerson(LocalPlayerSettingsStorage.localInstance.localPlayerSettings.thirdPerson);

        cameraOffset = standCameraOffset;
    }


    public void OnMouseLook()
    {

    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

        RaycastHit hit;

        if (Physics.Raycast(ray, hitInfo: out hit, 200))
        {
            if (hit.collider.tag == "Player")
            {
                Player player = hit.collider.GetComponentInParent<Player>();

                if (player != null)
                {
                    //lets not repeat whats been done
                    if (currentPlayerAboveInfo != player.playerAbove)
                    {
                        if (currentPlayerAboveInfo != null)
                            currentPlayerAboveInfo.gameObject.SetActive(false);

                        currentPlayerAboveInfo = player.playerAbove;
                        currentPlayerAboveInfo.gameObject.SetActive(true);
                    }
                }
                else
                    ClearAbovePlayerSelection(); //is/this
            }
            else
                ClearAbovePlayerSelection(); //this/is
        }
        else
            ClearAbovePlayerSelection(); //retarded


        float mouseX = look.x * Time.fixedDeltaTime;
        float mouseY = look.y * Time.fixedDeltaTime;

        if (lockYRot)
        {
            //Quaternion slopeRot = Quaternion.FromToRotation(transform.up, movement.floorNormal);

            //transform.rotation = Quaternion.Slerp(transform.rotation, slopeRot * transform.rotation, rotToFloor * Time.fixedDeltaTime);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
        }

        else
        { 
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 85f);

            transform.localRotation = Quaternion.Euler(xRotation + currentOffset.x, currentOffset.y, currentOffset.z);
        }

        movement.transform.Rotate(Vector3.up * mouseX);  

        lastPos = transform.position;

        if (Mathf.Abs(movement.moveInput.x) > 0.1f || Mathf.Abs(movement.moveInput.y) > 0.1f)
        {
            //Player is moving
            bobTime += Time.deltaTime * bobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, cameraOffset.y + Mathf.Sin(bobTime) * bobAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            bobTime = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, cameraOffset.y, Time.deltaTime * bobSpeed), cameraOffset.z);
        }

        if (Vector3.Distance(Vector3.zero, currentOffset) > 0.15f)
            currentOffset += (Vector3.zero - currentOffset).normalized * rotResetSpeed * Time.deltaTime;
    }

    public void Crouch(bool crouch)
    {
        if (crouch)
        {
            cameraOffset.x = crouchCameraOffset.x;
            cameraOffset.y = crouchCameraOffset.y;
        }
        else
        {
            cameraOffset.x = standCameraOffset.x;
            cameraOffset.y = standCameraOffset.y;
        }

    }

    void ClearAbovePlayerSelection()
    {
        if (currentPlayerAboveInfo != null)
            currentPlayerAboveInfo.gameObject.SetActive(false);

        currentPlayerAboveInfo = null;
    }


    public void Shake(float amount = 1)
    {
        //better than 2 if statements (but thats probs what the Clamp fuction does so000.
        amount = Mathf.Clamp(amount, 0.15f, 1.8f); //stop BIG SHAKEs (only at hungry hacks)

        Vector3 orignalPosition = cameraOffset;

        Tween.Shake(transform, orignalPosition, Vector3.one * magnatude * amount, shakeDuration, 0);
    }

    public void Dead(bool isDead)
    {
        deadCollider.enabled = isDead;
    }

    public void SetThirdPerson(bool thirdPerson)
    {
        if (thirdPerson)
            cameraOffset.z = -5;
        else
            cameraOffset.z = 0;
    }
}
