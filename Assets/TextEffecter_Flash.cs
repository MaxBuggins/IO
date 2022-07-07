using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TextEffecter_Flash : TextEffecter
{

    public string[] texts;
    private int currentIndex;

    void Start()
    {
        
    }


    void Update()
    {
        if (NetworkTime.time > lastHackTime + hackTime)
        {
            textMesh.text = "Only: " + LevelManager.instance.raceTimeRemaining.ToString("00:00") + " Remaining " + LevelManager.instance.players[0].userName + " = " + LevelManager.instance.players[0].bestTime;

            //else
            //textMesh.text = "No Race Recorderd or Running";

            timeSinceScroll = hackDuration - 1;
            Invoke(nameof(ResetText), hackDuration);
            return;
        }

        if (scrollSpeed < 0)
            return;

        timeSinceScroll += Time.deltaTime;

        if (timeSinceScroll > scrollSpeed)
        {
            currentIndex++;
            textMesh.text = texts[currentIndex];

            if (currentIndex >= texts.Length - 1)
                currentIndex = -1;

            timeSinceScroll = 0;
        }
    }
}
