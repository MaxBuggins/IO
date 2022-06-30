using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAboveInfo : MonoBehaviour
{

    public TextMeshPro textBox;

    public void ChangeText(string text)
    {
        textBox.text = text;
    }

    public void ChangeColour(Color32 colour)
    {
        textBox.color = colour; //God i hate american cOLoR, do they even eat fish?
    }


    void Update()
    {
        var lookPos = transform.position - Player.localInstance.transform.position;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
