using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_PickColour : MonoBehaviour
{
    private Image image;
    public PlayerAboveInfo AboveInfo;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void PickColour()
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.primaryColour = image.color;

        if(AboveInfo != null)
        {
            AboveInfo.ChangeColour(image.color);
        }
    }
}
