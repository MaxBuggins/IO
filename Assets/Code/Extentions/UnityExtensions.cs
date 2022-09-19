using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extra stuff to be used in all projects
/// Some Anon suggested this and i think it is a smart idear
/// <summary>
public static class UnityExtensions
{
    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static string colourToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color hexToColour(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    public static AudioSource PlayAudioAt(AudioClip clip, Vector3 pos, float volume = 1, float spaitalBlend = 0)
    {
        GameObject tempObj = new GameObject("TempAudio");
        tempObj.transform.position = pos; // set its position

        var aSource = tempObj.AddComponent<AudioSource>(); // add an audio source

        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = spaitalBlend;
                             
        aSource.Play(); // start the sound

        Object.Destroy(tempObj, clip.length); // destroy object after clip duration

        return aSource; // return the AudioSource reference
    }
}

