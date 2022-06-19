using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDelay : MonoBehaviour
{
    public float delay = 1;

    public bool destory;
    public bool detatchChildren;


    void Start()
    {
        Invoke(nameof(Action), delay);
    }

    void Action()
    {
        if (detatchChildren)
            transform.DetachChildren();

        if (destory)
            Destroy(this);
    }
}
