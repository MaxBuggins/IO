using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public static LevelManager instance;
    public float respawnDelay = 1;
    //public int[] spawnWeaponIDs; //all possible weapons that can be spawned with

    public AudioClip backgroundMusic;

    public float minSightHeight = -399;

    private List<Transform> spawnPoints = new List<Transform>();

    private AudioSource audioSource;
    private AudioDistortionFilter audioDistortion;

    //private UI_Main playerUI;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        audioSource = GetComponent<AudioSource>();
        audioDistortion = GetComponent<AudioDistortionFilter>();

        //playerUI = FindObjectOfType<UI_Main>();


        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    //public int GetSpawnWeaponID()
    //{
        //return (spawnWeaponIDs[Random.Range(0, spawnWeaponIDs.Length)]);
    //}
}
