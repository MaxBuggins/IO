using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Score : UI_Base
{
    public TextMeshProUGUI score;

    // Update is called when base says so
    public override void Update()
    {
        score.text = Player.localInstance.GetScore().ToString(); //thank you me

        base.Update();
    }
}
