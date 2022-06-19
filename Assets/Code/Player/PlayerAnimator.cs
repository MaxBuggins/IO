using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(DirectionalSprite))] //fancy
//[RequireComponent(typeof(Player))] //fancy ERRRROR DONT DO THIS IF PARENT
public class PlayerAnimator : MonoBehaviour
{
    public Texture2D idleStandSprite;
    public Texture2D idleCrouchSprite;

    private DirectionalSprite directionalSprite;
    private Player player;


    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();
        player = GetComponentInParent<Player>();
    }


    void Update()
    {
        if (player.crouching == true)
        {
            SetSprites(idleCrouchSprite);
        }
        else
        {
            SetSprites(idleStandSprite);
        }
    }

    public void SetSprites(Texture2D texture)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/"+texture.name);

        int i = 0;
        foreach (Sprite sprite in sprites)
        {
            directionalSprite.directionalSprites[i] = sprite;
            i++;
        }

        directionalSprite.SetSprite(directionalSprite.currentSpriteIndex);
    }
}
