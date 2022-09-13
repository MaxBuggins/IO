using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UI_ScoreBoard : UI_Base
{
    public GameObject scoreRowPrefab;

    public List<UI_ScoreRow> scoreRows = new List<UI_ScoreRow>();

    private void OnEnable()
    {
        Update();
    }


    public override void Update()
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

            rowRectTrans.sizeDelta = new Vector2(500, 50);
            rowRectTrans.anchoredPosition = new Vector2(rowRectTrans.anchoredPosition.x, -100 - (count-1) * 50);
        }

    }
}
