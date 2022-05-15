using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirectionalSprite))] //fancy still

public class PlayerCorpse : MonoBehaviour
{
    public Sprite[] deathSprites;
    public float[] frameDelays;

    private int frameIndex = 0;

    private DirectionalSprite directionalSprite;

    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();

        Invoke(nameof(NextFrame), frameDelays[0]);
    }

    void NextFrame()
    {
        directionalSprite.render.sprite = deathSprites[frameIndex];
        frameIndex += 1;

        if (frameIndex >= deathSprites.Length)
            return;

        Invoke(nameof(NextFrame), frameDelays[frameIndex]);

    }
}
