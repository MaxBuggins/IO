using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Velocity : UI_Base
{
    public TextMeshProUGUI vel;
    public TextMeshProUGUI mag;

    // Update is called when base says so
    public override void Update()
    {
        vel.text = "" + Player.localInstance.playerMovement.velocity;
        mag.text = "" + Player.localInstance.playerMovement.velocity.magnitude;

        base.Update();
    }
}
