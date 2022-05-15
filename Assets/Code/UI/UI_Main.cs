using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public static UI_Main instance;

    public UI_Base[] bases;

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
        foreach(UI_Base uI_Base in bases)
        {
            uI_Base.Update();
        }
    }
}
