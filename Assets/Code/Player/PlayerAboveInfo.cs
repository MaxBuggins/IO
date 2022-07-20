using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAboveInfo : MonoBehaviour
{

    public TextMeshPro textBox;

    public void ChangeText(string name)
    {
        textBox.text = name;
    }

    public void ChangeText(string name, float time)
    {
        if(time >= 0)
            textBox.text = name + "\n" + (time * 100).ToString("00:00");
        else
        {
            textBox.text = name + "\n Attemptless";
        }
    }

    public void ChangeColour(Color colour)
    {
        textBox.color = colour; //God i hate american cOLoR, do they even eat fish?
    }

    public void ChangeColour(Color32 colour)
    {
        textBox.color = colour; //God i hate american cOLoR, do they even eat fish?
    }


    void Update()
    {
        var lookPos = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
