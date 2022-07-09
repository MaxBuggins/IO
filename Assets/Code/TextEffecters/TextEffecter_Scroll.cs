using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffecter_Scroll : TextEffecter
{
    public float scrollSpeed = -1;
    protected float timeSinceScroll = 0;

    void Update()
    {
        if (CheckHack())
            return;

        if (scrollSpeed < 0)
            return;

        timeSinceScroll += Time.deltaTime;

        if (timeSinceScroll > scrollSpeed)
        {
            textMesh.text = textMesh.text.Substring(1, textMesh.text.Length - 1) + textMesh.text.Substring(0, 1);

            timeSinceScroll = 0;
        }
    }
}
