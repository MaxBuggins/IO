using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mute : MonoBehaviour
{
    public bool muted;

    public Image mutedOverlayImage;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }


    public void OnClick()
    {
        muted = !muted;

        mutedOverlayImage.enabled = muted;
    }
}
