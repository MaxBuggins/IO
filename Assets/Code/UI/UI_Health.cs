using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Health : UI_Base
{
    public TextMeshProUGUI textMesh;
    public Image barImage;

    public int segmentSize = 6;

    // Update is called when base says so
    public override void Update()
    {

        textMesh.text = "" + Player.localInstance.health;

        float fillAmount = (float)Player.localInstance.health / (float)Player.localInstance.maxHealth;
        fillAmount = Mathf.Ceil(fillAmount * segmentSize) / segmentSize;

        barImage.fillAmount = fillAmount;

        base.Update();
    }

    public override void RefreshColour(Color primaryColour)
    {
        barImage.color = primaryColour;
        textMesh.color = primaryColour;
    }
}
