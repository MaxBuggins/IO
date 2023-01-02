using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.IO;
using System.Text;
using TMPro;

public class MediaPickUp : MonoBehaviour, Interact
{
    [Header("Interaction")]
    public float maxInteractDistanceValue;
    public float maxInteractDistance
    {
        get { return maxInteractDistanceValue; }
        set { maxInteractDistanceValue = value; }
    }

    public string fileName;
    public byte password = 0;

    public float pickUpDuration = 0.5f;
    public AnimationCurve pickUpCurve;

    public Vector3 rotOffset;


    public void Interact(float distance)
    {
        if (distance > maxInteractDistance)
            return;

        Player.localInstance.playerMovement.enabled = false;

        Tween.Position(transform, Camera.main.transform.position, pickUpDuration, 0, pickUpCurve);
        Tween.Rotation(transform, Camera.main.transform.eulerAngles + rotOffset, pickUpDuration * 0.6f, 0, pickUpCurve);

        //Player.localInstance.playerMovement.enabled = false;

        UI_Main.instance.DisplayHint(true, 1, 2);
        
        Player.localInstance.StartCoroutine(RealTimeDecrypt());

        StartCoroutine(FinishPickup(pickUpDuration));
    }

    IEnumerator RealTimeDecrypt()
    {
        string text = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, fileName));

        GameObject alertObject = UI_Main.instance.CreateAlert(text, 9, Color.green, 4, 0, 3, Vector2.zero);

        TextMeshProUGUI alerttmPro = alertObject.GetComponentInChildren<TextMeshProUGUI>();

        yield return new WaitForEndOfFrame();

        if (text.StartsWith(":Decoded:") == false)
        {
            StringBuilder decryptedString = new StringBuilder();
            int i = 0;
            foreach (char c in text)
            {
                decryptedString.Append((char)(c - password));
                alerttmPro.text = text.Remove(0, decryptedString.Length) + decryptedString.ToString(); //text.Remove(0, decryptedString.Length) + 

                if (i > 12) //decryptedString.Length / 10
                {
                    //yield return new WaitForEndOfFrame();
                    i = 0;
                }

                i++;
            }

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, fileName), decryptedString.ToString());
            text = decryptedString.ToString();
        }

        if (Application.isEditor)
        {
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, fileName), StringEncryptor.Encrypt(text, password));
        }
    }

    public IEnumerator FinishPickup(float duration)
    {
        yield return new WaitForSeconds(duration);

        Player.localInstance.playerMovement.enabled = true;


        //GetComponent<Renderer>().enabled = false;
        Destroy(gameObject); //bye bye
    }
}

public static class StringEncryptor
{
    public static string Encrypt(string input, int key)
    {
        StringBuilder encryptedString = new StringBuilder();

        foreach (char c in input)
        {
            encryptedString.Append((char)(c + key));         
        }

        return encryptedString.ToString();
    }

    public static string Decrypt(string input, int key)
    {
        StringBuilder decryptedString = new StringBuilder();

        foreach (char c in input)
        {
            decryptedString.Append((char)(c - key));
        }

        return decryptedString.ToString();
    }
}