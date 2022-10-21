using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class UI_Sticker : MonoBehaviour
{
    public Vector2 stickerOffset;
    public Vector2 randomStickerRotationRange;

    public Sprite[] stickers;


    public float rotateDuration = 0;
    public AnimationCurve rotateAnimationCurve;

    public RectTransform rectTransform;

    void OnEnable()
    {
        Quaternion endRot = Quaternion.Euler(0, 0, Random.Range(randomStickerRotationRange.x, randomStickerRotationRange.y));

        if (rotateDuration > 0)
        {
            transform.eulerAngles = new Vector3(90, 0, 0);
            Tween.LocalRotation(transform, endRot, rotateDuration, 0, rotateAnimationCurve);
        }
        else
            transform.rotation = endRot;

        GetComponent<Image>().sprite = stickers[Random.Range(0, stickers.Length)];

        Vector2 rectRange = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        rectRange /= 2;
        rectRange -= stickerOffset;

        Vector2 stickerPos = new Vector2(Random.Range(-rectRange.x, rectRange.x), Random.Range(-rectRange.y, rectRange.y));
        transform.localPosition = rectTransform.rect.center + stickerPos;
    }
}
