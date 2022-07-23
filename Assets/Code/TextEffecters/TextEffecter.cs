using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class TextEffecter : MonoBehaviour
{
    protected float lastHackTime = 3;
    public float hackTime = 13;
    public float hackDuration = 6;

    protected string orginalText;

    protected TextMeshPro textMesh;
    
    void Awake()
    {
        if (NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
            Destroy(this);

        textMesh = GetComponent<TextMeshPro>();
        orginalText = textMesh.text;
    }

    protected bool CheckHack()
    {
        bool isHacked = NetworkTime.time > lastHackTime + hackTime;

        if (isHacked)
        {
            textMesh.text = "Only: " + LevelManager.instance.raceTimeRemaining.ToString("00:00") + " Remaining " + LevelManager.instance.players[0].userName + " = " + LevelManager.instance.players[0].bestTime;

            Invoke(nameof(ResetText), hackDuration);
        }

        return (isHacked);
    }

    protected void ResetText()
    {
        lastHackTime = (float)NetworkTime.time;
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
