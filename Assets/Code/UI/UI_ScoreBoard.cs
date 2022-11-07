using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
using Steamworks;
#endif

public class UI_ScoreBoard : UI_Base
{
    public bool levelScoreboard = true;


    public GameObject scoreRowPrefab;

    public List<UI_ScoreRow> scoreRows = new List<UI_ScoreRow>();

    private void OnEnable()
    {
        SteamLeaderboard.DownloadLeaderboard();
        Update();
    }



    public override void Update()
    {
        if (levelScoreboard)
            UpdateLevelScoreboard();
        
        else
            UpdateSteamScoreboard();
    }


    void UpdateLevelScoreboard()
    {
        //Have the right number of player rows
        while (scoreRows.Count != LevelManager.instance.players.Count)
        {
            if (scoreRows.Count > LevelManager.instance.players.Count)
            {
                UI_ScoreRow row = scoreRows[scoreRows.Count - 1];
                scoreRows.Remove(row);
                Destroy(row.gameObject);
            }

            if (scoreRows.Count < LevelManager.instance.players.Count)
            {
                UI_ScoreRow row = Instantiate(scoreRowPrefab, transform).GetComponent<UI_ScoreRow>();
                scoreRows.Add(row);
            }
        }

        int count = 0;

        foreach (UI_ScoreRow row in scoreRows)
        {
            row.banner.color = LevelManager.instance.players[count].primaryColour;

            row.userName.text = LevelManager.instance.players[count].userName;

            switch (LevelManager.instance.gameMode)
            {
                case (LevelManager.GameMode.surf):
                    {
                        if (LevelManager.instance.players[count].bestTime > 0)
                        {
                            row.bestTime.text = (LevelManager.instance.players[count].bestTime * 100).ToString("00:00");
                        }
                        else
                            row.bestTime.text = "No Attempt";

                        break;
                    }

                case (LevelManager.GameMode.deathmatch):
                    {
                        if (LevelManager.instance.players[count].kills > 0)
                        {
                            row.kills.text = ("" + LevelManager.instance.players[count].kills);
                        }
                        else
                            row.bestTime.text = "Virgin";

                        break;
                    }
            }


            RectTransform rowRectTrans = row.GetComponent<RectTransform>();

            int even = 1;
            if (count % 2 == 0)
                even = -1;

            row.transform.eulerAngles = Vector3.up * 2.5f * even;
            count += 1;

            rowRectTrans.sizeDelta = new Vector2(300, 50);
            rowRectTrans.anchoredPosition = new Vector2(rowRectTrans.anchoredPosition.x, -100 - (count - 1) * 50);
        }

    }

    void UpdateSteamScoreboard()
    {
        //Have the right number of player rows
        SteamLeaderboard_t steamLeaderboard = SteamLeaderboard.currentLeaderboard;
        SteamLeaderboardEntries_t steamLeaderboardEntrys = SteamLeaderboard.currentLeaderboardEntrys;

        if (steamLeaderboardEntrys == null)
        {
            print("download");
            SteamLeaderboard.DownloadLeaderboard();
            return;
        }

        int count = Mathf.Clamp(SteamUserStats.GetLeaderboardEntryCount(steamLeaderboard), 0, 10);

        while (scoreRows.Count != count)
        {
            if (scoreRows.Count > count)
            {
                UI_ScoreRow row = scoreRows[scoreRows.Count - 1];
                scoreRows.Remove(row);
                Destroy(row.gameObject);
            }

            if (scoreRows.Count < count)
            {
                UI_ScoreRow row = Instantiate(scoreRowPrefab, transform).GetComponent<UI_ScoreRow>();
                scoreRows.Add(row);
            }
        }

        count = 0;


        foreach (UI_ScoreRow row in scoreRows)
        {
            LeaderboardEntry_t leaderboardEntry;
            bool ret = SteamUserStats.GetDownloadedLeaderboardEntry(steamLeaderboardEntrys, count, out leaderboardEntry, null, 0);
            //print("SteamUserStats.GetDownloadedLeaderboardEntry(" + steamLeaderboardEntrys+ ", " + 0 + ", " + "out LeaderboardEntry" + ", " + null + ", " + 0 + ") : " + ret + " -- " + leaderboardEntry);
            //row.banner.color = SteamUserStats.Down;

            //print(leaderboardEntry.m_cDetails);
            

            row.userName.text = "" + SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);

            switch (LevelManager.instance.gameMode)
            {
                case (LevelManager.GameMode.surf):
                    {
                        row.bestTime.text = (leaderboardEntry.m_nScore.ToString("00:00"));
                        break;
                    }

                case (LevelManager.GameMode.deathmatch):
                    {
                        if (LevelManager.instance.players[count].kills > 0)
                        {
                            row.kills.text = ("" + leaderboardEntry.m_nScore);
                        }
                        else
                            row.bestTime.text = "Virgin";

                        break;
                    }
            }


            RectTransform rowRectTrans = row.GetComponent<RectTransform>();

            int even = 1;
            if (count % 2 == 0)
                even = -1;

            row.transform.eulerAngles = Vector3.up * 2.5f * even;
            count += 1;

            rowRectTrans.sizeDelta = new Vector2(300, 50);
            rowRectTrans.anchoredPosition = new Vector2(rowRectTrans.anchoredPosition.x, -100 - (count - 1) * 50);
        }

    }
}

