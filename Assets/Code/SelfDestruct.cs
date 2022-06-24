using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float destoryDelay = 1;


    private void Start()
    {
        Invoke(nameof(DestroySelf), destoryDelay);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
