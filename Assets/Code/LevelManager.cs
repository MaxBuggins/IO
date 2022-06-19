using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : MonoBehaviour
{
    public float respawnDelay = 1;
    public int[] spawnWeaponIDs; //all possible weapons that can be spawned with

    private List<Transform> spawnPoints = new List<Transform>();

    private AudioSource audioSource;
    private AudioDistortionFilter audioDistortion;

    //private UI_Main playerUI;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioDistortion = GetComponent<AudioDistortionFilter>();

        //playerUI = FindObjectOfType<UI_Main>();
    
    }

    public int GetSpawnWeaponID()
    {
        return (spawnWeaponIDs[Random.Range(0, spawnWeaponIDs.Length)]);
    }
}
