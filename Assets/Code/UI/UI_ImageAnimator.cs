using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_ImageAnimator : MonoBehaviour
{
    public float frameDuration = 0.2f;
    private float nextFrameTime;

    public Sprite[] sprites;
    private int currentIndex;


    private Image imageRenderer;

    void Start()
    {
        imageRenderer = GetComponent<Image>();

        nextFrameTime = Time.time + frameDuration;
    }

    void Update()
    {
        if(Time.time > nextFrameTime)
        {
            nextFrameTime = Time.time + frameDuration;

            currentIndex++;

            if(currentIndex >= sprites.Length)
            {
                currentIndex = 0;
            }

            imageRenderer.sprite = sprites[currentIndex];
        }
    }
}
