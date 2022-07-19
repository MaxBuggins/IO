using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageShake : MonoBehaviour
{
    public float frequency = 0.5f;
    private float timeTillShake = 0;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        timeTillShake -= Time.deltaTime;
        if(timeTillShake < 0)
        {
            rectTransform.anchoredPosition = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
            timeTillShake = frequency;
        }
    }
}
