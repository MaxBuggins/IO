using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private int currentFrame = 0;
    public float frameDelay = 1;

    public Texture2D[] idleStandSprite;

    public bool crouching;
    public Texture2D[] idleCrouchSprite;

    private DirectionalSprite directionalSprite;


    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();

        InvokeRepeating(nameof(NextFrame), 0, 1.0f);
    }


    void NextFrame()
    {
        currentFrame++;
        if (crouching == true)
        {
            if (currentFrame >= idleCrouchSprite.Length)
                currentFrame = 0;

            SetSprites(idleCrouchSprite[currentFrame]);
        }
        else
        {
            if (currentFrame >= idleStandSprite.Length)
                currentFrame = 0;

            SetSprites(idleStandSprite[currentFrame]);
        }
    }

    public void SetSprites(Texture2D texture)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/" + texture.name);

        int i = 0;
        foreach (Sprite sprite in sprites)
        {
            directionalSprite.directionalSprites[i] = sprite;
            i++;
        }

        directionalSprite.SetSprite(directionalSprite.currentSpriteIndex);
        directionalSprite.spriteCount = i;
        directionalSprite.sortAngle = 360 / i;
    }
}
