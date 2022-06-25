using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    [HideInInspector] public static LevelManager instance;
    public float respawnDelay = 1;
    //public int[] spawnWeaponIDs; //all possible weapons that can be spawned with

    public AudioClip backgroundMusic;

    public float minSightHeight = -399;

    private List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private float raceDuration = 60;
    [HideInInspector] public float raceTimeRemaining;
    [SyncVar(hook = nameof(OnRaceChange))]
    public int currentLevelRace = -1;
    public MasterCheckPoint[] levelRaces;


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

    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerChangeRace();
    }

    private void Update()
    {
        raceTimeRemaining -= Time.deltaTime;

        if (isServer == false)
            return;

        if (raceTimeRemaining < 0)
            ServerChangeRace();
    }

    [Server]
    public void ServerChangeRace()
    {
        if (levelRaces.Length < 2)
            return; //not enough races


        int newRaceIndex = Random.Range(0, levelRaces.Length);
        while (newRaceIndex == currentLevelRace)
        {
            newRaceIndex = Random.Range(0, levelRaces.Length);
        }

        if (currentLevelRace > -1)
            levelRaces[currentLevelRace].ServerActivateCheckPoints(false);

        currentLevelRace = newRaceIndex;
        //levelRaces[currentLevelRace].active = true;
        levelRaces[currentLevelRace].ServerActivateCheckPoints(true);

        raceTimeRemaining = raceDuration;
    }

    public void OnRaceChange(int oldRaceIndex, int newRaceIndex)
    {
        //if (oldRaceIndex > -1)
            //levelRaces[oldRaceIndex].active = false;

        //levelRaces[newRaceIndex].gameObject.SetActive(true);
    }

    //public int GetSpawnWeaponID()
    //{
        //return (spawnWeaponIDs[Random.Range(0, spawnWeaponIDs.Length)]);
    //}
}
