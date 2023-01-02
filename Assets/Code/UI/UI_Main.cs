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

    [SerializeField] private Image screenImage;
    [SerializeField] private Image hurtOverlay;

    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject touchUI;
    
    //who even cares if this one is public
    [SerializeField] public GameObject scoreBoardUI;
    public UI_Crosshaire UI_Crosshaire;

    [SerializeField] private GameObject[] alertObjects;
    [SerializeField] private GameObject winScreenPrefab;

    [SerializeField] private Transform hintPos;
    [SerializeField] private Transform hiddenHintPos;
    [SerializeField] private GameObject[] hintObjects;


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

    public void OnHurt(float damageRatio)
    {
        Color currentColour = hurtOverlay.color;
        currentColour.a += damageRatio;
        hurtOverlay.color = currentColour;

        currentColour.a = 0;
        Tween.Color(hurtOverlay, currentColour, 0.8f, 0.4f);
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

    public void OnPassRing()
    {
        Instantiate(winScreenPrefab, transform);
    }

    public void DisplayHint(bool display, int index, float duration = 0)
    {
        if (display)
        {
            if(duration == 0)
                hintObjects[index].transform.position = Vector3.MoveTowards(hintObjects[index].transform.position, hintPos.position, 1500 * Time.deltaTime);

            else
            {
                Tween.Position(hintObjects[index].transform, hintPos.position, duration * 0.25f, 0, AnimationCurve.EaseInOut(0, 0, 1, 1));
                Invoke(nameof(HideHint), duration * 0.75f);
            }
        }
        else
        {
            hintObjects[index].transform.position = Vector3.MoveTowards(hintObjects[index].transform.position, hiddenHintPos.position, 300 * Time.deltaTime);
        }
    }

    public void HideHint()
    {
        Tween.Position(hintObjects[1].transform, hiddenHintPos.position, 2f, 0, AnimationCurve.EaseInOut(0, 0, 1, 1));
    }

    public GameObject CreateAlert(string text, float fontSize, Color fontColour, float duration = 2, float delay = 0, int alertObjIndex = 0, Vector2? moveDirection = null)
    {
        if (moveDirection == null)
        {
            moveDirection = Vector2.down * 600;
        }

        GameObject createdAlertObject = Instantiate(alertObjects[alertObjIndex], transform);

        createdAlertObject.GetComponent<SelfDestruct>().destoryDelay = duration;

        TextMeshProUGUI createdAlertObjectTMPro = createdAlertObject.GetComponentInChildren<TextMeshProUGUI>();
        createdAlertObjectTMPro.text = text;
        createdAlertObjectTMPro.fontSize = fontSize;
        createdAlertObjectTMPro.color = fontColour;

        Tween.AnchoredPosition(createdAlertObject.GetComponent<RectTransform>(), moveDirection.Value, duration, delay);

        return (createdAlertObject);
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
            UI_Crosshaire.crossHair.color = Color.clear;
        else
            UI_Crosshaire.crossHair.color = colour;

        UI_Crosshaire.crossHair.sprite = crosshairSprites[index];
        
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
