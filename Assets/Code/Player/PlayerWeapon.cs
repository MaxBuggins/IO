using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))] //fancy still
public class PlayerWeapon : NetworkBehaviour
{
	//Tooltips are for profesionals
	[Tooltip("Closer to 0 the more accurate")]
	[Range(0, 90)] public float accuracy = 0;

	[Tooltip("Spawn Position relative to the players position")]
	private Vector3 spawnOffset;
	[Tooltip("Spawn Position relative to the players position when standing")]
	public Vector3 standSpawnOffset;
	[Tooltip("Spawn Position relative to the players position when crouching")]
	public Vector3 crouchSpawnOffset;

	public GameObject prmaryObject;

	public GameObject secondaryObject;
	private float timeSinceSecondary = 0;
	public float secondaryCoolDown = 2;


	[SerializeField] private GameObject masterCheckPointPrefab;

	private bool primaryHeld = false;

	protected Player player;
	public Controls controls;

    private void Start()
    {
		player = GetComponent<Player>();

		timeSinceSecondary = secondaryCoolDown;
    }

    private void Update()
    {
		timeSinceSecondary += Time.deltaTime;
    }

    public override void OnStartLocalPlayer()
	{
		controls = new Controls();

		controls.Play.Primary.performed += funny => UsePrimary();
		controls.Play.Primary.performed += funny => primaryHeld = true;
		controls.Play.Primary.canceled += funny => primaryHeld = false;

		controls.Play.Secondary.performed += funny => UseSecondary();

		controls.Enable();
	}

	[Command]
	public void CmdShootGun(Vector3 position, Vector3 rotationAim)
	{
		//dead players cant shoot
		if (player.health <= 0)
			return;

		 //if(Vector3.Distance(position, transform.position) > 1.5f)//1.5 is the max distance delay that will be allowed
			//position = transform

			//randomise bullet spread
			Vector3 offset = new Vector3(0, Random.Range(-accuracy, accuracy), 0);
		Quaternion shootRot = Quaternion.Euler(rotationAim + offset);

		if (player.crouching)
			spawnOffset = crouchSpawnOffset;
		else
			spawnOffset = standSpawnOffset;


		GameObject spawned = Instantiate(prmaryObject, position + spawnOffset, shootRot, null);

		//applys damage multiplyer for gun
		Hurtful hurtful = spawned.GetComponent<Hurtful>();
		if (hurtful != null)
		{
			hurtful.ignor = player;
			//hurtful.damage = (int)((float)hurtful.damage * currentGun.dmgMultiplyer);
		}

		NetworkServer.Spawn(spawned);
	}

	[Command]
	public void CmdCreateRing(Quaternion rotation)
	{
		//dead players cant shoot
		if (player.health <= 0)
			return;

		if (timeSinceSecondary < secondaryCoolDown) //not enough time progressed
			return;

		if(player.ownedRace != null)
			if (player.ownedRace.finished == true)
				return;


		timeSinceSecondary = 0;

		if (player.ownedRace == null)
		{
			//I really liked this method but Networking be like NOOOOOO client needs it tooo WAHHHhhhhhh
			//GameObject masterCheckPointObject = new GameObject("MasterCheckPoint");
			//MasterCheckPoint masterCheckPoint = masterCheckPointObject.AddComponent<MasterCheckPoint>();

			GameObject masterCheckPointObject = Instantiate(masterCheckPointPrefab, null);
			NetworkServer.Spawn(masterCheckPointObject);

			MasterCheckPoint masterCheckPoint = masterCheckPointObject.GetComponent<MasterCheckPoint>();

			masterCheckPoint.finished = false;
			masterCheckPoint.colour = player.primaryColour;
			player.ownedRace = masterCheckPoint;
			masterCheckPoint.netIdentity.AssignClientAuthority(player.connectionToClient);
		}

		if (player.crouching)
			spawnOffset = crouchSpawnOffset;
		else
			spawnOffset = standSpawnOffset;


		player.ownedRace.ServerCreateCheckPoint(transform.position, rotation);

		//GameObject spawned = Instantiate(secondaryObject, transform.position + spawnOffset, rotation, player.ownedRace.transform);
		//CheckPoint checkPoint = spawned.GetComponent<CheckPoint>();

		//if (checkPoint == null)                    WHO CARES not me AT THE MOMENT
			//return; //Brugh whdy you do this

		//checkPoint.masterCheckPoint = player.ownedRace;

		//player.ownedRace.checkPoints.Add(checkPoint);
		//player.ownedRace.RefreshCheckPoints();


		//NetworkServer.Spawn(spawned);
	}

	void UsePrimary()
    {
		CmdShootGun(transform.position, PlayerCamera.localInstance.transform.eulerAngles);
    }

	void UseSecondary()
	{
		if (timeSinceSecondary < secondaryCoolDown) //not enough time progressed
			return;

		CmdCreateRing(PlayerCamera.localInstance.transform.rotation);

		if (isClientOnly)
			timeSinceSecondary = 0;
	}
}
