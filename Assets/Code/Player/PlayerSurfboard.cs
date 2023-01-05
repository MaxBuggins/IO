using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSurfboard : MonoBehaviour
{
    [Header("Variables")]
    [Range(0,1)] public float slerpAmount = 0.5f;
    public float surfboardJumpFlipSpeed = 100;

    public LayerMask groundMask;
    public float maxRayDistance = 0.8f;

    public Vector3 addVector;

    private Vector3 orginalRotation;
    private float lastPlayerRotY;

    [Header("Refrences")]
    public Transform primaryGroundTrans;
    public Transform collisionParticals;
    [SerializeField] private Renderer surfboardRenderer;
    [SerializeField] private Material fp_surfboard_material;
    public Player player;

    private void Start()
    {
        orginalRotation = transform.localEulerAngles;
        lastPlayerRotY = player.transform.eulerAngles.y;


        Texture surfboardTexture;
        surfboardTexture = surfboardRenderer.material.mainTexture;
        surfboardRenderer.material = fp_surfboard_material;
        surfboardRenderer.material.mainTexture = surfboardTexture;
    }


    private void LateUpdate()
    {
        Color whiteTrans = Color.white;

        whiteTrans.a = Mathf.InverseLerp(80, 0, Camera.main.transform.eulerAngles.x) + 0.05f;

        //whiteTrans.a
        //whiteTrans.a = Camera.main.rot

        surfboardRenderer.material.color = whiteTrans;

        RaycastHit hit;

        if (Physics.Raycast(primaryGroundTrans.position, Vector3.down, out hit, maxRayDistance * 5, groundMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance > maxRayDistance)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(orginalRotation), slerpAmount / 2);
                collisionParticals.gameObject.SetActive(false);
            }
            else
            {
                Quaternion delta = Quaternion.FromToRotation(transform.up, hit.normal);
                delta.ToAngleAxis(out float angle, out Vector3 axis);
                Quaternion halfDelta = Quaternion.AngleAxis(0.5f * angle, axis);
                Quaternion newRot = Quaternion.Slerp(transform.rotation, halfDelta * transform.rotation.normalized, slerpAmount);

                //newRot.eulerAngles = new Vector3(newRot.eulerAngles.x, player.playerCamera.transform.eulerAngles.y, newRot.eulerAngles.z);

                transform.rotation = newRot;

                collisionParticals.gameObject.SetActive(true);
            }

        }
        else
        {
            float playerRotAmount = (player.transform.eulerAngles.y - lastPlayerRotY) * Time.deltaTime;

            transform.localEulerAngles += Vector3.forward * playerRotAmount * surfboardJumpFlipSpeed;

            lastPlayerRotY = player.transform.eulerAngles.y;
        }
    }
}
