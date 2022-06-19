using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Health : UI_Base
{
    public Text textMesh;
    public Image barImage;

    // Update is called when base says so
    public override void Update()
    {

        textMesh.text = "" + Player.localInstance.health;
        barImage.fillAmount = (float)Player.localInstance.health / (float)Player.localInstance.maxHealth;

        base.Update();
    }
}
