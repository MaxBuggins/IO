using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pixelplacement;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem.UI;

public class UI_Main : MonoBehaviour
{
    public static UI_Main instance;

    public UI_Base[] bases;

    public Sprite[] crosshairSprites;
    [SerializeField] private Image crossHair;

    [SerializeField] private Image screenImage;

    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject touchUI;
    //who even cares if this one is public
    [SerializeField] public GameObject scoreBoardUI;

    [SerializeField] private GameObject[] alertObjects;

    public Canvas canvas;
    [HideInInspector] public InputSystemUIInputModule inputSystemUIInput;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

#if UNITY_IOS
        touchUI.SetActive(true);
#endif

        canvas = GetComponent<Canvas>();
        inputSystemUIInput = GetComponentInChildren<InputSystemUIInputModule>();
    }

    private void Start()
    {
        SetCrosshairImage(LocalPlayerSettingsStorage.localInstance.localPlayerSettings.crosshairIndex, LocalPlayerSettingsStorage.localInstance.localPlayerSettings.crosshairColour);
    }

    public void UIUpdate()
    {
        if (Player.localInstance == null)
            return;

        LevelManager.instance.RefreshPlayerList();

        foreach (UI_Base uI_Base in bases)
        {
            uI_Base.Update();
        }

        if (Player.localInstance.health <= 0)
        {
            gameOverUI.SetActive(true);
            playerUI.SetActive(false);
            screenImage.color = new Color(0.706f, 0.125f, 0.165f, 0.2f);
        }
        else
        {
            playerUI.SetActive(true);
            gameOverUI.SetActive(false);
            screenImage.color = Color.clear;
        }
    }

    public void RefreshColour()
    {
        foreach (UI_Base uI_Base in bases)
        {
            uI_Base.RefreshColour(Player.localInstance.primaryColour);
        }
    }

    public void OnPause(bool pause)
    {
        playerUI.SetActive(!pause);
        pauseUI.SetActive(pause);

        if (pause)
        {
            Cursor.lockState = CursorLockMode.None;
            screenImage.color = new Color(0.135f, 0.125f, 0.706f, 0.2f);
        }
        else
        {
            screenImage.color = Color.clear;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ShowScoreboard(bool show)
    {
        UIUpdate();

        playerUI.SetActive(!show);
        scoreBoardUI.SetActive(show);

        if (show)
        {
            screenImage.color = new Color(0.2f, 1f, 0.4f, 0.2f);
        }
        else
        {
            screenImage.color = Color.clear;
        }
    }

    public void CreateAlert(string text, float fontSize, Color fontColour, float duration = 2, float delay = 0, int alertObjIndex = 0)
    {
        GameObject createdAlertObject = Instantiate(alertObjects[alertObjIndex], transform);

        createdAlertObject.GetComponent<SelfDestruct>().destoryDelay = duration;

        TextMeshProUGUI createdAlertObjectTMPro = createdAlertObject.GetComponent<TextMeshProUGUI>();
        createdAlertObjectTMPro.text = text;
        createdAlertObjectTMPro.fontSize = fontSize;
        createdAlertObjectTMPro.color = fontColour;

        Tween.AnchoredPosition(createdAlertObject.GetComponent<RectTransform>(), Vector2.down * 600, duration, delay);
    }

    public void ChangeScreenColour(Color colour)
    {
        screenImage.color = colour;
    }

    public void TemparyChangeScreenColour(Color colour, float duration = -1)
    {
        screenImage.color = colour;

        if (duration > 0)
            Tween.Color(screenImage, Color.clear, duration, 0);

    }

    public void SetCrosshairImage(int index, Color colour)
    {
        index--;

        if (index < 0)
            crossHair.color = Color.clear;
        else
            crossHair.color = colour;

        crossHair.sprite = crosshairSprites[index];
        
    }

    public void Disconect()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {

            NetworkManager.singleton.StopHost();

        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {

            NetworkManager.singleton.StopClient();

        }
        // stop server if server-only
        else if (NetworkServer.active)
        {

            NetworkManager.singleton.StopServer();

        }
    }
}
