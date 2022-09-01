using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType { none, melee, shoot }

[CreateAssetMenu(fileName = "WeaponObject", menuName = "WeaponObject", order = 1)]
public class WeaponObject : ScriptableObject
{
	public int rareity = 1; // 1 - 5
	public WeaponType weaponType;

	[Header("Primary")]
	public float primaryDelay = 0.1f;
	public float primaryCooldown = 0.2f;
	public GameObject spawnPrimaryObject;
	public int primarySpawnCount = 1;

	[Header("Secondary")]
	public float secondaryDelay = 0;
	public float secondaryCoolDown = 0;
	public GameObject spawnSecondaryObject;

	//Tooltips are for profesionals
	[Tooltip("Spawn Position relative to the players position when standing")]
	public Vector3 standSpawnOffset;
	[Tooltip("Spawn Position relative to the players position when crouching")]
	public Vector3 crouchSpawnOffset;


	[Header("Gun Stuff")]
	[Tooltip("Closer to 0 the more accurate")]
	[Range(0, 90)] public float accuracy = 0;
	public int maxAmmo;


	[Header("Refrences")]
	public GameObject firstPersonObject;
	public GameObject thirdPersonObject;
}
