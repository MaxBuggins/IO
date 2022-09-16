using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class UI_Crosshaire : MonoBehaviour
{
    public Image crossHair;
    public Image reloadCircle;
    public TextMeshProUGUI ammoCount;

    private Color reloadCircleColour;

    void Start()
    {
        reloadCircleColour = reloadCircle.color;
        reloadCircle.fillAmount = 1;
        reloadCircle.color = Color.clear;
        ammoCount.enabled = false;
    }

    public void Reload(float duration)
    {
        reloadCircle.color = reloadCircleColour;

        reloadCircle.fillAmount = 0;

        Tween.Value(0f, 1f, HandleFillChange, duration, 0);
    }

    void HandleFillChange(float value)
    {
        reloadCircle.fillAmount = value;
        
        if(value >= 1)
        {
            reloadCircle.color = Color.clear;
        }
    }

    public void SetAmmo(int amount)
    {
        if (ammoCount.text == amount.ToString())
            return;

        //Tween.LocalScale(ammoCount.transform, Vector3.)
        ammoCount.text = amount.ToString();
    }
}
