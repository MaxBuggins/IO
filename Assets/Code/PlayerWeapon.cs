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
	public Vector3 spawnOffset;

	public GameObject projectile;


	private bool primaryHeld = false;



	protected Player player;
	private Controls controls;

    private void Start()
    {
		player = GetComponent<Player>();
    }

	public override void OnStartLocalPlayer()
	{
		controls = new Controls();

		controls.Play.Primary.performed += funny => UsePrimary();
		controls.Play.Primary.performed += funny => primaryHeld = true;
		controls.Play.Primary.canceled += funny => primaryHeld = false;

		controls.Enable();
	}

	[Command]
	public void CmdShootGun(Vector3 rotationAim)
	{

		//dead players cant shoot
		if (player.health <= 0)
			return;

		//randomise bullet spread
		Vector3 offset = new Vector3(0, Random.Range(-accuracy, accuracy), 0);
		Quaternion shootRot = Quaternion.Euler(rotationAim + offset);


		GameObject spawned = Instantiate(projectile, transform.position + spawnOffset, shootRot, null);

		//applys damage multiplyer for gun
		Hurtful hurtful = spawned.GetComponent<Hurtful>();
		if (hurtful != null)
		{
			//hurtful.damage = (int)((float)hurtful.damage * currentGun.dmgMultiplyer);
		}

		NetworkServer.Spawn(spawned);
	}

    void UsePrimary()
    {
		CmdShootGun(transform.eulerAngles);
    }
}
