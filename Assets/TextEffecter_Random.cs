using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffecter_Random : TextEffecter
{
    public float insertDelay = 0.25f;
    private float timeSinceInsert = 0.5f;
    public string stringRange = "You Should Eat Fish Now Thanks \n";

    void Start()
    {
        
    }

    void Update()
    {
        if (CheckHack())
            return;

        timeSinceInsert += Time.deltaTime;

        if(timeSinceInsert > insertDelay)
        {
            string[] words = stringRange.Split(' ');
            textMesh.text += words[Random.Range(0, words.Length)] +"\n";
        }
    }
}
