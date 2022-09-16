using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ScoreRow : MonoBehaviour
{
    //public Image icon;
    public Image banner;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI kills;
    //public TextMeshProUGUI assits;
    //public TextMeshProUGUI deaths;
    //public TextMeshProUGUI score;

    public Sprite[] backSprites;

    private void Start()
    {
        banner.sprite = backSprites[Random.Range(0, backSprites.Length)];

        switch (LevelManager.instance.gameMode)
        {
            case (LevelManager.GameMode.surf):
                {
                    bestTime.gameObject.SetActive(true);
                    break;
                }

            case (LevelManager.GameMode.deathmatch):
                {
                    kills.gameObject.SetActive(true);
                    break;
                }
        }
    }
}
