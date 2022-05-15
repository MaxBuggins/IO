using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    private void Awake()
    {
        enabled = false; //only update on UI_Mains Command
    }

    public virtual void Update()
    {
        
    }
}
