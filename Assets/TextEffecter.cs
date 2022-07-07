using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class TextEffecter : MonoBehaviour
{
    public float scrollSpeed = -1;
    protected float timeSinceScroll = 0;


    protected float lastHackTime = 0;
    public float hackTime = 5;
    public float hackDuration = 6;

    private string orginalText;

    protected TextMeshPro textMesh;
    
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        orginalText = textMesh.text;
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
            textMesh.text = textMesh.text.Substring(1, textMesh.text.Length - 1) + textMesh.text.Substring(0, 1);

            timeSinceScroll = 0;
        }
    }

    protected void ResetText()
    {
        lastHackTime = (float)NetworkTime.time;
        timeSinceScroll = 1;
        textMesh.text = orginalText;
    }
}

/*            currentScrollLength++;

            if(currentScrollLength > maxScrollLength)
            {
                currentScrollLength = 0;
            }    
            string whiteSpace = new string(' ', currentScrollLength);

            textMesh.text = whiteSpace + text;
*/
