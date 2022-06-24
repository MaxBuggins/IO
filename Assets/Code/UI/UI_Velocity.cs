using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Velocity : UI_Base
{
    public TextMeshProUGUI vel;
    public TextMeshProUGUI mag;
    public TextMeshProUGUI groundedTimeText;

    // Update is called when base says so
    public void FixedUpdate()
    {
        if (Player.localInstance == null)
            return;

        string velText = "" + Mathf.Round(Player.localInstance.playerMovement.velocity.x * 10.0f) / 10.0f
            + "" + Mathf.Round(Player.localInstance.playerMovement.velocity.y * 10.0f) / 10.0f
            + "" + Mathf.Round(Player.localInstance.playerMovement.velocity.z * 10.0f) / 10.0f;

        vel.text = velText;

        mag.text = (Mathf.Round(
            Player.localInstance.playerMovement.velocity.magnitude * 10.0f) / 10.0f).ToString("000.00");

        groundedTimeText.text = (Mathf.Round(
            Player.localInstance.timeSinceGrounded * 10.0f) / 10.0f).ToString("000:00");
    }
}
