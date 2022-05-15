using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDelay : MonoBehaviour
{
    public float delay = 1;

    public bool unParent = false;
    public bool destoryParent;


    void Start()
    {
        Invoke(nameof(Action), delay);
    }

    void Action()
    {
        Transform parent = transform.parent;
        if (unParent)
            transform.parent = null;

        if (destoryParent)
            Destroy(parent);
    }
}
