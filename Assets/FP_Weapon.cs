using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FP_Weapon : MonoBehaviour
{
    private Animator animator;
    public GameObject trail;

    void Start()
    {
        animator = GetComponent<Animator>();   
    }

    public void onPrimary()
    {
        animator.SetTrigger("Primary");
    }
}
