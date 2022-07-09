using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TextEffecter_Flash : TextEffecter
{
    //public Vector2 flashDelayRange = new Vector2(0.1f, 1f);
    public AnimationCurve flashDelayRange;
    private float timeTillFlash;

    private Color orginalColour;
    private Color orginalColourClear;

    private void Start()
    {
        orginalColour = textMesh.color;
        orginalColourClear = textMesh.color;
        orginalColourClear.a = 0.1f;

        timeTillFlash = flashDelayRange.Evaluate(Random.value);
    }

    protected void Update()
    {
        CheckHack();

        timeTillFlash -= Time.deltaTime;

        if(timeTillFlash < 0)
        {
            timeTillFlash = flashDelayRange.Evaluate(Random.value);

            if (textMesh.color == orginalColourClear)
                textMesh.color = orginalColour;
            else
            {
                textMesh.color = orginalColourClear;
                timeTillFlash *= 0.5f;
            }
        }
    }
}
