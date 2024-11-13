using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSettings : ScriptableObject
{
    [Header("User Stuff")]
    public string userName;

    public Color32 primaryColour;
    public int patternIndex;
    public int stickerIndex;


    [Header("Settings Stuff")]
    public float mouseSensativity = 15f;

    public int crosshairIndex = 2;
    public Color crosshairColour = Color.white;

    public bool showOwnHat = false;
    public bool enableRbs = true;
    public bool displayTVs = true;

    public bool thirdPerson = false;

    public PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Multiply;

}
