using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.IO;
using System.Text;
using TMPro;

public class MediaPickUp : MonoBehaviour
{
    public string fileName;
    public byte password = 0;

    [Header("Interaction")]
    public float maxInteractDistance = 2.5f;

    public float pickUpDuration = 0.5f;
    public AnimationCurve pickUpCurve;

    public Vector3 rotOffset;

    [Header("Dycrption effect")]
    public float decryptSpeed = 0.01f;


    void Start()
    {

    }

    void Update()
    {

    }

    public void Interact(float distance)
    {
        if (distance > maxInteractDistance)
            return;


        Tween.Position(transform, Camera.main.transform.position, pickUpDuration, 0, pickUpCurve);
        Tween.Rotation(transform, Camera.main.transform.eulerAngles + rotOffset, pickUpDuration * 0.6f, 0, pickUpCurve);

        //Player.localInstance.playerMovement.enabled = false;

        StartCoroutine(RealTimeDecrypt());
    }

    IEnumerator RealTimeDecrypt()
    {
        string text = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, fileName));

        GameObject alertObject = UI_Main.instance.CreateAlert(text, 9, Color.green, 4, 0, 3, Vector2.zero);

        TextMeshProUGUI alerttmPro = alertObject.GetComponentInChildren<TextMeshProUGUI>();

        if (text.StartsWith(":Decoded:") == false)
        {
            StringBuilder decryptedString = new StringBuilder();

            foreach (char c in text)
            {
                decryptedString.Append((char)(c - password));
                alerttmPro.text =  text.Remove(0, decryptedString.Length) + decryptedString.ToString();

                if(Random.Range(0, 10) == 0)
                    yield return new WaitForEndOfFrame();
            }

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, fileName), decryptedString.ToString());
        }

        if (Application.isEditor)
        {
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, fileName), StringEncryptor.Encrypt(text, password));
        }
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