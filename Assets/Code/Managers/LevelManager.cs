using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    [HideInInspector] public static LevelManager instance;

    [Header("Level Propertys")]
    public float respawnDelay = 1;
    //public int[] spawnWeaponIDs; //all possible weapons that can be spawned with

    public AudioClip backgroundMusic;

    public float minSightHeight = -399;

    [Header("Races")]
    public MasterCheckPoint[] levelRaces;
    [SerializeField] private float raceDuration = 60;
    [SyncVar] private double raceStartTime = 0;

    [HideInInspector] public float raceTimeRemaining;
    [SyncVar(hook = nameof(OnRaceChange))]
    public int currentLevelRace = -1;


    [Header("Stats")]
    [HideInInspector] public List<Player> players = new List<Player>();
    public SyncList<string> lastRoundResults = new SyncList<string>();


    [Header("Level Refrences")]
    private List<Transform> spawnPoints = new List<Transform>();
    public GameObject rbsParentObject;


    [Header("Unity Stuff")]
    private AudioSource audioSource;
    private AudioDistortionFilter audioDistortion;

    //private UI_Main playerUI;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        audioSource = GetComponent<AudioSource>();
        audioDistortion = GetComponent<AudioDistortionFilter>();

        rbsParentObject.SetActive(LocalPlayerSettingsStorage.localInstance.localPlayerSettings.enableRbs);

        //set up SyncList
        lastRoundResults.Callback += OnLastResultsUpdated;

        RefreshPlayerList();
    }

    private void Start()
    {
        if (isServerOnly) //I dont want the server to obnoixuly blast subway song through the racks? 
            return;

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
        //casting Network Time down to float will literrnly offset accuraccy by 8ms a day [Brugh]
        //aka WHO CARES I DONT PLAN ON ADDING 20DAY RACES BRUVY

        raceTimeRemaining = raceDuration - (float)(NetworkTime.time - raceStartTime);


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
        {
            levelRaces[currentLevelRace].ServerActivateCheckPoints(false);

            lastRoundResults.Clear();
            RefreshPlayerList();

            for (int p = 0; p < players.Count; p++) //we pusing P
            {
                if (players[p].bestTime >= 0)
                {
                    if (p == 0)
                        players[p].hatIndex = 1;

                    lastRoundResults.Add(players[p].userName + " | " + (players[p].bestTime * 100).ToString("00:00") + "*" + UnityExtensions.colourToHex(players[p].primaryColour));
                }

                players[p].ClearRace();
            }
        }

        currentLevelRace = newRaceIndex;
        //levelRaces[currentLevelRace].active = true;
        levelRaces[currentLevelRace].ServerActivateCheckPoints(true);

        raceStartTime = NetworkTime.time;
    }

    public void OnRaceChange(int oldRaceIndex, int newRaceIndex)
    {
        if (oldRaceIndex == -1)
            return;

    }

    void OnLastResultsUpdated(SyncList<string>.Operation operation, int index, string oldResult, string newResult)
    {
        switch (operation)
        {
            case SyncList<string>.Operation.OP_ADD:
                {
                    string[] texts = lastRoundResults[index].Split('*');
                    float fontSize = 24;
                    float delay = 0;

                    switch (index)
                    {
                        case (0):
                            {
                                fontSize = 48;
                                delay = 1f;
                                break;
                            }
                        case (1):
                            {
                                fontSize = 32;
                                delay = 0.5f;
                                break;
                            }
                    }

                    if(!isServerOnly)
                        UI_Main.instance.CreateAlert(texts[0], fontSize, UnityExtensions.hexToColour(texts[1]), 7, delay, alertObjIndex: 2);

                    break;
                }
        }
    }


    public void RefreshPlayerList()
    {
        players.Clear();
        players.AddRange(FindObjectsOfType<Player>());
        players.Sort((p1, p2) => p1.bestTime.CompareTo(p2.bestTime));

        for (int i = 0; i < players.Count; i++) //move no attempts to back
        {
            Player player = players[i];

            if (player.bestTime < 0)
            {
                players.RemoveAt(i);
                players.Add(player);
            }
        }
    }

}
