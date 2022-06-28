using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Username : UI_Base
{
    public TextMeshProUGUI usernameText;



    public override void Update()
    {
        if (Player.localInstance == null)
            return;

        usernameText.text = Player.localInstance.userName;
        usernameText.color = Player.localInstance.primaryColour;
    }
}
