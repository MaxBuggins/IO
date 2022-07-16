using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacteristicsObject", menuName = "CharacteristicsObject", order = 1)]
public class CharacteristicsObject : ScriptableObject
{
    public string className;

    public AudioClip[] lauphSounds;
    public AudioClip[] hurtSounds;
    public AudioClip[] deathSounds;

    public PlayerCallout[] playerCallouts;
}


public enum CalloutType { lauph, medic, yes, no, debug } // Music Generas

[System.Serializable]
public class PlayerCallout
{
    public CalloutType calloutCatagory;

    public AudioClip[] clips;
}
