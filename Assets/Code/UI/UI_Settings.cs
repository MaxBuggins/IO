using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    public Slider sensativitySlider;
    public Toggle showOwnHatToggle;
    public Toggle enableRbsToggle;

    private void Start()
    {
        sensativitySlider.value = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.mouseSensativity;
        showOwnHatToggle.isOn = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.showOwnHat;
        enableRbsToggle.isOn = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.enableRbs;
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

    public void SetCrosshairIndex(int index)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.crosshairIndex = index;

        if (UI_Main.instance != null)
        {
            UI_Main.instance.SetCrosshairImage(index, LocalPlayerSettingsStorage.localInstance.localPlayerSettings.crosshairColour);
        }
    }

    public void SetEnableRbs(bool enable)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.enableRbs = enable;

        if (LevelManager.instance != null)
        {
            LevelManager.instance.rbsParentObject.SetActive(enable);
        }


    }
}
