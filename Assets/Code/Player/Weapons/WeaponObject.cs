using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType { none, melee, shoot }

[CreateAssetMenu(fileName = "WeaponObject", menuName = "WeaponObject", order = 1)]
public class WeaponObject : ScriptableObject
{
	public int rareity = 1; // 1 - 5

	public float primaryDelay = 0.1f;
	public float primaryCooldown = 0.2f;
	public GameObject spawnPrimaryObject;

	[Tooltip("Spawn Position relative to the players position when standing")]
	public Vector3 standSpawnOffset;
	[Tooltip("Spawn Position relative to the players position when crouching")]
	public Vector3 crouchSpawnOffset;


	//Tooltips are for profesionals
	//[Tooltip("Closer to 0 the more accurate")]
	//[Range(0, 90)] public float accuracy = 0;



	[Header("Refrences")]
	public GameObject firstPersonObject;
	public GameObject thirdPersonObject;
}
