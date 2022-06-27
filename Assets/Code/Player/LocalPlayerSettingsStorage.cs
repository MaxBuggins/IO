using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSettingsStorage : MonoBehaviour
{
    public static LocalPlayerSettingsStorage localInstance;
    public LocalPlayerSettings localPlayerSettings;

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

    public void SetUserName(string userName)
    {
        localPlayerSettings.userName = userName;
    }

    public void SetShowOwnHat(bool show)
    {
        localPlayerSettings.showOwnHat = show;
    }
}
