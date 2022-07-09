using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSettings : ScriptableObject
{
    public string userName;

    public Color32 primaryColour;
    public Color32 secondaryColour;
    public Color32 thirdryColour;

    public string killMessage;

    public float mouseSensativity = 15f;

    public int crosshairIndex = 2;
    public Color crosshairColour = Color.white;

    public bool showOwnHat = false;
    public bool enableRbs;

    public PhysicMaterialCombine bounceCombine = PhysicMaterialCombine.Multiply;
}
