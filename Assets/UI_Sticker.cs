using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sticker : MonoBehaviour
{
    public Vector2 stickerOffset;
    public Vector2 randomStickerRotationRange;

    public Sprite[] stickers;

    public RectTransform rectTransform;

    void OnEnable()
    {
        GetComponent<Image>().sprite = stickers[Random.Range(0, stickers.Length)];

        Vector2 rectRange = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        rectRange /= 2;
        rectRange -= stickerOffset;

        transform.localPosition = rectTransform.rect.center + new Vector2(Random.Range(-rectRange.x, rectRange.x), Random.Range(-rectRange.y, rectRange.y));
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(randomStickerRotationRange.x, randomStickerRotationRange.y));
    }
}
