using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalPlayerSettings", menuName = "LocalPlayerSettings", order = 1)]
public class LocalPlayerSettings : ScriptableObject
{
    public string userName;

    public Color32 primaryColour;
    public Color32 secondaryColour;
    public Color32 thirdryColour;

    public string killMessage;
}
