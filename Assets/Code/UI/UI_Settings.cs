using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    public Slider sensativitySlider;
    public Toggle showOwnHatToggle;
    public Toggle enableRbsToggle;
    public Toggle thirdPersonToggle;

    private void Start()
    {
        sensativitySlider.value = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.mouseSensativity;
        showOwnHatToggle.isOn = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.showOwnHat;
        thirdPersonToggle.isOn = LocalPlayerSettingsStorage.localInstance.localPlayerSettings.thirdPerson;
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

    public void SetThirdPerson(bool thirdPerson)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.thirdPerson = thirdPerson;

        if(Player.localInstance != null)
        {
            Player.localInstance.playerCamera.SetThirdPerson(thirdPerson);

            if(thirdPerson)
                Player.localInstance.playerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            else
                Player.localInstance.playerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    public void SetDisplayTV(bool enable)
    {
        LocalPlayerSettingsStorage.localInstance.localPlayerSettings.displayTVs = enable;
        foreach (UnityEngine.Video.VideoPlayer videoPlayer in FindObjectsOfType<UnityEngine.Video.VideoPlayer>())
        {
            videoPlayer.enabled = false;
        }
    }
}
