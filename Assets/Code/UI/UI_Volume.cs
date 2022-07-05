using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UI_Volume : UI_Base
{
    public TextMeshProUGUI volumeText;

    public AudioMixerGroup audioGroup;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        float value;
        audioGroup.audioMixer.GetFloat(audioGroup.name, out value);
        slider.value = value;

        volumeText.text = value.ToString("F1") + " dB";
    }

    public void UpdateValue(float value)
    {
        if (value <= -50)
        {
            audioGroup.audioMixer.SetFloat(audioGroup.name, -100);
            volumeText.text = "Muted";
        }
        else
        {
            audioGroup.audioMixer.SetFloat(audioGroup.name, value);
            volumeText.text = value.ToString("F1") + " dB";
        }
    }
}

