using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public static UI_Main instance;

    public UI_Base[] bases;

    public GameObject playerUI;
    public GameObject gameOverUI;
    public GameObject pauseUI;

    public Canvas canvas;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        UIUpdate();
    }

    public void UIUpdate()
    {
        if (Player.localInstance == null)
            return;

        foreach (UI_Base uI_Base in bases)
        {
            uI_Base.Update();
        }

        if (Player.localInstance.health <= 0)
        {
            gameOverUI.SetActive(true);
            playerUI.SetActive(false);
        }
        else
        {
            playerUI.SetActive(true);
            gameOverUI.SetActive(false);
        }
    }

    public void OnPause(bool pause)
    {
        playerUI.SetActive(!pause);
        pauseUI.SetActive(pause);

        if (pause)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public void ChangeSensativity(float sensativity)
    {
        Player.localInstance.playerCamera.mouseLookSensitivty = sensativity;
    }

}
