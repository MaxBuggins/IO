using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerWeapon : NetworkBehaviour
{
	//private WeaponObject weaponObject;

	[SyncVar(hook = nameof(OnWeaponChanged))] 
	public int weaponIndex = -1;
	public WeaponObject weaponObject;

	[Tooltip("Stored Player Rotation")]
	private Vector3 currentRotation;
	[Tooltip("Stored Player Position")]
	private Vector3 currentPosition;
	[Tooltip("The network time when the player last succesfully used there primary attack")]
	private double timeSinceLastPrimary;
	[Tooltip("Time the server primary attack is behing the client")]
	private double primaryTimeOffset;

	[Tooltip("The network time when the player last succesfully used there secondary action")]
	private double timeSinceLastSecondary;
	[Tooltip("Time the server secondary action is behing the client")]
	private double secondaryTimeOffset;

	[SyncVar][SerializeField] private int ammoRemaining;

	private GameObject fpWeaponObject;
	private GameObject thirdPersonObject;
	private FP_Weapon fpWeapon;

	private Player player;

    private void Awake()
    {
		player = GetComponent<Player>();
	}

	public void OnWeaponChanged(int oldWeapon, int newWeapon)
	{
		weaponObject = LevelManager.instance.weapons[newWeapon];

		if (fpWeaponObject != null)
			Destroy(fpWeaponObject);

		if (thirdPersonObject != null)
			Destroy(thirdPersonObject);

		thirdPersonObject = Instantiate(weaponObject.thirdPersonObject, player.playerAnimator.rightHand);

		ammoRemaining = weaponObject.maxAmmo;

		if (isLocalPlayer)
		{
			//THIS IS ASS CODE DO NOT ACCEPT ASS CODE I AM LAZY
			MeshRenderer meshRenderer = thirdPersonObject.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

			foreach (Transform child in thirdPersonObject.transform)
			{
				MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();

				if(childMeshRenderer != null)
					childMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			}

			fpWeaponObject = Instantiate(weaponObject.firstPersonObject, player.playerCamera.transform.GetChild(0));
			fpWeapon = fpWeaponObject.GetComponentInChildren<FP_Weapon>();
		}

	}

    #region primary
    public void OnPrimary()
	{
		if (weaponIndex < 0)
			return;

		if (NetworkTime.time < timeSinceLastPrimary + weaponObject.primaryCooldown)
			return;

		if (weaponObject.maxAmmo > 0 && ammoRemaining <= 0)
			return;

		player.playerAnimator.TriggerPrimaryAttack();
		fpWeapon.onPrimary();

		CmdPrimary(NetworkTime.time, PlayerCamera.localInstance.transform.position, PlayerCamera.localInstance.transform.eulerAngles);

		timeSinceLastPrimary = NetworkTime.time; //MUST OCCUR AFTER CMDPRIMARY for host
	}


	[Command]
	public void CmdPrimary(double sentTime, Vector3 position, Vector3 rotation)
    {
		currentRotation = rotation;
		currentPosition = position;

		if (sentTime < timeSinceLastPrimary + weaponObject.primaryCooldown)
			return;

		timeSinceLastPrimary = sentTime;

		if (player.health <= 0) //dead men dont shoot
			return;

		if (weaponObject.maxAmmo > 0)
		{
			if(ammoRemaining <= 0)
				return;

			ammoRemaining--;
		}

		RpcUsePrimary();

		primaryTimeOffset = NetworkTime.time - sentTime;

		float delay = (float)(weaponObject.primaryDelay - primaryTimeOffset);
		primaryTimeOffset -= weaponObject.primaryDelay;


		Invoke(nameof(PrimaryAction), delay);
	}

	[Server]
	void PrimaryAction()
	{
		//dead players cant act
		if (player.health <= 0)
			return;

		Vector3 spawnOffset;

		if (player.crouching)
			spawnOffset = weaponObject.crouchSpawnOffset;
		else
			spawnOffset = weaponObject.standSpawnOffset;

		Vector3 worldOffset = transform.rotation * spawnOffset;
		Vector2 cameraOffset = player.playerCamera.standCameraOffset;

		worldOffset += new Vector3(cameraOffset.x, cameraOffset.y);

		Vector3 fakeRotation = currentRotation;
		fakeRotation.y = transform.rotation.eulerAngles.y;

		if (weaponObject.weaponType == WeaponType.melee)
			SpawnChildObject(weaponObject.spawnPrimaryObject, transform.position + worldOffset, fakeRotation);

		if (weaponObject.weaponType == WeaponType.shoot)
			ShootProjectile(weaponObject.spawnPrimaryObject, transform.position + worldOffset, fakeRotation, weaponObject.accuracy);
	}


	[ClientRpc]
	public void RpcUsePrimary()
	{
		if (isLocalPlayer)
			return;

		player.playerAnimator.TriggerPrimaryAttack();
	}

    #endregion

    #region secondary
    public void OnSecondary()
	{
		if (weaponIndex < 0)
			return;

		if (NetworkTime.time < timeSinceLastSecondary + weaponObject.secondaryCoolDown)
			return;

		fpWeapon.onSecondary();

		CmdSecondary(NetworkTime.time, PlayerCamera.localInstance.transform.position, PlayerCamera.localInstance.transform.eulerAngles);

	}

	[Command]
	void CmdSecondary(double sentTime, Vector3 position, Vector3 rotation)
	{
		currentRotation = rotation;
		currentPosition = position;

		if (sentTime < timeSinceLastSecondary + weaponObject.secondaryCoolDown)
			return;

		timeSinceLastSecondary = sentTime;

		if (player.health <= 0) //dead men dont shoot
			return;


		secondaryTimeOffset = NetworkTime.time - sentTime;

		float delay = (float)(weaponObject.secondaryDelay - secondaryTimeOffset);
		secondaryTimeOffset -= weaponObject.secondaryDelay;

		timeSinceLastPrimary = NetworkTime.time + weaponObject.secondaryCoolDown;

		Invoke(nameof(SecondaryAction), delay);
	}

	void SecondaryAction()
    {
		//dead players cant act
		if (player.health <= 0)
			return;

		Vector3 spawnOffset;

		if (player.crouching)
			spawnOffset = weaponObject.crouchSpawnOffset;
		else
			spawnOffset = weaponObject.standSpawnOffset;

		Vector3 worldOffset = transform.rotation * spawnOffset;
		Vector2 cameraOffset = player.playerCamera.standCameraOffset;

		worldOffset += new Vector3(cameraOffset.x, cameraOffset.y);

		Vector3 fakeRotation = currentRotation;
		fakeRotation.y = transform.rotation.eulerAngles.y;


		if (weaponObject.weaponType == WeaponType.melee)
			SpawnChildObject(weaponObject.spawnSecondaryObject, transform.position + worldOffset, fakeRotation);

		else if (weaponObject.weaponType == WeaponType.shoot)
			Reload();
	}

	[Server]
	void Reload()
    {
		ammoRemaining = weaponObject.maxAmmo;
	}

	#endregion

	[Server]
	public void SpawnChildObject(GameObject gameObject, Vector3 position, Vector3 rotation)
	{
		GameObject spawned;

		spawned = Instantiate(gameObject, position, Quaternion.Euler(rotation), transform);

		Hurtful hurtful = spawned.GetComponent<Hurtful>();
		if (hurtful != null)
		{
			hurtful.ignor = player;
		}

		//NetworkServer.Spawn(spawned);
	}

	[Server]
	public void ShootProjectile(GameObject gameObject, Vector3 position, Vector3 rotationAim, float accuracy)
	{
		//randomise bullet spread
		Vector3 offset = new Vector3(Random.Range(-accuracy, accuracy) / 2, Random.Range(-accuracy, accuracy), 0);
		Quaternion shootRot = Quaternion.Euler(rotationAim + offset);

		GameObject spawned = Instantiate(gameObject, position, shootRot, null);


		Projectile projectile = spawned.GetComponent<Projectile>();
		if (projectile != null)
		{
			projectile.ignor = player;
			//projectile.timeSinceStart = (float)primaryTimeOffset;
		}


		NetworkServer.Spawn(spawned);
	}
}