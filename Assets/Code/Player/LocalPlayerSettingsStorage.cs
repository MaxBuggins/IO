using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSettingsStorage : MonoBehaviour
{
    public static LocalPlayerSettingsStorage localInstance;
    public LocalPlayerSettings localPlayerSettings;

    public LocalPlayerSettings defaultlocalPlayerSettings;

    private void Awake()
    {
        if (localInstance == null)
            localInstance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    public void SetPrimaryColour(Color color)
    {
        localPlayerSettings.primaryColour = new Color(color.r, color.g, color.b);
    }

    public void SetPattern(int index)
    {
        localPlayerSettings.patternIndex = index;
    }

    public void SetUserName(string userName)
    {
        localPlayerSettings.userName = userName;
    }

    public void SetBounceType(int index)
    {
        localPlayerSettings.bounceCombine = (PhysicMaterialCombine)index;
    }

    public void SetDisplayTV(bool enable) //broken
    {
        localPlayerSettings.displayTVs = enable;
        foreach (UnityEngine.Video.VideoPlayer videoPlayer in FindObjectsOfType<UnityEngine.Video.VideoPlayer>())
        {
            videoPlayer.enabled = enable;
        }
    }

    //I dont like this solution
    public void resetPlayerSettings()
    {
        localPlayerSettings.mouseSensativity = defaultlocalPlayerSettings.mouseSensativity;

        localPlayerSettings.crosshairIndex = defaultlocalPlayerSettings.crosshairIndex;
        localPlayerSettings.crosshairColour = defaultlocalPlayerSettings.crosshairColour;

        localPlayerSettings.showOwnHat = defaultlocalPlayerSettings.showOwnHat;
        localPlayerSettings.enableRbs = defaultlocalPlayerSettings.enableRbs;

        localPlayerSettings.thirdPerson = defaultlocalPlayerSettings.thirdPerson;

        localPlayerSettings.bounceCombine = defaultlocalPlayerSettings.bounceCombine;
    }
}
