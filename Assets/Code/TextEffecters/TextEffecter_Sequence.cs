using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffecter_Sequence : TextEffecter
{
    public float textDuration = 0.7f;
    private float timeSinceTextChange;

    public string[] texts;
    private int currentIndex;

    void Start()
    {
        
    }


    void Update()
    {
        if (CheckHack())
            return;

        timeSinceTextChange += Time.deltaTime;

        if(timeSinceTextChange > textDuration)
        {
            currentIndex++;
            textMesh.text = texts[currentIndex];

            if (currentIndex >= texts.Length - 1)
                currentIndex = -1;

            timeSinceTextChange = 0;
        }
    }
}
