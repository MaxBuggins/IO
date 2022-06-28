using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    public Slider sensativitySlider;
    public Toggle showOwnHatToggle;

    private void Start()
    {
        sensativitySlider.value = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.mouseSensativity;
        showOwnHatToggle.isOn = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.showOwnHat;
    }

    public void ToggleOnandOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


    public void ChangeSensativity(float sensativity)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.mouseSensativity = sensativity;

        if (Player.localInstance != null)
            Player.localInstance.playerCamera.mouseLookSensitivty = sensativity;
    }


    public void SetShowOwnHat(bool show)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.showOwnHat = show;

        if (Player.localInstance != null)
            Player.localInstance.SetLocalHatVisability();
    }

    public void SetShowCrosshair()
    {
        //LocalPlayerSettingsStorage.localInstance.localPlayerSettings.showCrosshair;

        //if (Player.localInstance != null)
            //Player.localInstance.playerCamera.
    }
}
