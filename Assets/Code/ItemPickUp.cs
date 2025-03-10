using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemPickUp : NetworkBehaviour
{
    [Header("Move Propertys")]
    public float sinSpeed;
    public float sinHeight;

    public float rotSpeed = 90;

    private Vector3 orginPos;

    [Header("PickUp Propertys")]
    public bool isActive = true;
    public bool respawn = true;
    public float respawnDelay;
    private float respawnTime;


    [Header("Item Effects")]
    [Range(0,1)] public float healthPercentage; //relative to player max health
    public int score;
    public int hatIndex = -1; //-1 means no hat
    public int weaponIndex = -1; //-1 is none


    [Header("Internals")]
    public GameObject renderObject;
    private Renderer render;
    private Collider trigger;

    private void Start()
    {
        render = GetComponent<Renderer>();
        trigger = GetComponent<Collider>();

        orginPos = transform.localPosition;
        orginPos += Vector3.up * sinHeight * 1.5f;
    }

    void Update()
    {

        if (isServer && isActive == false)
        {
            respawnTime += Time.deltaTime;
            if (respawnTime > respawnDelay)
            {
                isActive = true;
                RpcSetItem(true);
            }
        }

        float yPos = orginPos.y + Mathf.Sin(Time.time * sinSpeed) * sinHeight;

        transform.localPosition = new Vector3(orginPos.x, yPos, orginPos.z);

        transform.localEulerAngles = new Vector3(0, Time.time * rotSpeed, 0);
    }

    [ClientRpc]
    public void RpcSetItemPosition(Vector3 position)
    {
        transform.position = position;
        transform.eulerAngles = Vector3.zero;

        orginPos = position;
        orginPos += Vector3.up * sinHeight * 1.5f;
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (isActive == false)
            return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            if (player.health <= 0)
                return;

            if(healthPercentage != 0)
                player.Hurt((int)(-player.maxHealth * healthPercentage)); //makes player gain health (WACKY)

            player.bonusScore += score;

            if(hatIndex > 0) //debug hat not possible (He He Ha Ha)
            {
                player.hatIndex = hatIndex;
            }

            if(weaponIndex > -1)
            {
                ServerWeapon serverWeapon = player.GetComponent<ServerWeapon>();
                serverWeapon.OnWeaponChanged(serverWeapon.weaponIndex, weaponIndex);
                serverWeapon.weaponIndex = weaponIndex;
            }

            if (respawn)
            {
                respawnTime = 0;
                RpcSetItem(false);
            }
            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    [ClientRpc]
    void RpcSetItem(bool active)
    {
        if (render == null)
            renderObject.SetActive(active);
        else
            render.enabled = active;


        trigger.enabled = active;

        isActive = active;
    }
}