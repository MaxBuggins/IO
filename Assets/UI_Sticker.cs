using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sticker : MonoBehaviour
{
    public Vector2 randomStickerPositionRange;
    public Vector2 randomStickerRotationRange;

    public Sprite[] stickers;

    void Start()
    {
        GetComponent<Image>().sprite = stickers[Random.Range(0, stickers.Length)];

        randomStickerPositionRange /= 2;
        transform.localPosition = new Vector3(Random.Range(-randomStickerPositionRange.x, randomStickerPositionRange.x), Random.Range(-randomStickerPositionRange.y, randomStickerPositionRange.y), transform.localPosition.z);
        transform.localEulerAngles = new Vector3(0,0,Random.Range(randomStickerRotationRange.x, randomStickerRotationRange.y));
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, randomStickerPositionRange);
    }
}
