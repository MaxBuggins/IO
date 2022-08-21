using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerWeapon : NetworkBehaviour
{
	//private WeaponObject weaponObject;
	public WeaponObject weaponObject;

	[Tooltip("Spawn Position relative to the players position")]
	private Vector3 spawnOffset;
	private double timeSinceLastPrimary;

	private GameObject fpWeaponObject;
	private FP_Weapon fpWeapon;

	private Player player;

    private void Awake()
    {
		player = GetComponent<Player>();
	}

    private void OnEnable()
    {
		if(fpWeaponObject != null)
			Destroy(fpWeaponObject);

		var thirdPersonObject = Instantiate(weaponObject.thirdPersonObject, player.playerAnimator.rightHand);


		if (isLocalPlayer)
		{
			thirdPersonObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

			fpWeaponObject = Instantiate(weaponObject.firstPersonObject, player.playerCamera.transform.GetChild(0));
			fpWeapon = fpWeaponObject.GetComponent<FP_Weapon>();
		}	
    }

    public void OnPrimary()
	{
		if (enabled == false)
			return;

		if (NetworkTime.time < timeSinceLastPrimary + weaponObject.primaryCooldown)
			return;

		player.playerAnimator.TriggerPrimaryAttack();
		fpWeapon.onPrimary();

		CmdPrimary(NetworkTime.time);

		timeSinceLastPrimary = NetworkTime.time; //MUST OCCUR AFTER CMDPRIMARY for host
	}

	[Command]
	public void CmdPrimary(double sentTime)
    {
		if (sentTime < timeSinceLastPrimary + weaponObject.primaryCooldown)
			return;

		timeSinceLastPrimary = sentTime;

		if (player.health <= 0) //dead men dont shoot
			return;

		RpcUsePrimary();

		if (player.crouching)
			spawnOffset = weaponObject.crouchSpawnOffset;
		else
			spawnOffset = weaponObject.standSpawnOffset;


		Invoke(nameof(PrimaryAction), (float)(sentTime + weaponObject.primaryDelay - NetworkTime.time));
    }

    void PrimaryAction()
    {
		SpawnChildObject(weaponObject.spawnPrimaryObject, spawnOffset);
	}

	[ClientRpc]
	public void RpcUsePrimary()
	{
		if (isLocalPlayer)
			return;

		player.playerAnimator.TriggerPrimaryAttack();
	}

	public void SpawnChildObject(GameObject gameObject, Vector3 offset)
	{
		//dead players cant shoot
		if (player.health <= 0)
			return;

		GameObject spawned;

		spawned = Instantiate(gameObject, gameObject.transform.position + transform.position + offset, Quaternion.identity, transform);

		Hurtful hurtful = spawned.GetComponent<Hurtful>();
		if (hurtful != null)
		{
			hurtful.ignor = player;
		}

		NetworkServer.Spawn(spawned);
	}
}
