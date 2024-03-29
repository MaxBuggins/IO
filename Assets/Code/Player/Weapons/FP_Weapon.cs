using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FP_Weapon : MonoBehaviour
{
    public float speedMultiplyer = 1;

    private Animator animator;
    public GameObject fxObject;
    public Spawner ammoSpawner;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("Speed", speedMultiplyer);
    }

    public void onPrimary()
    {
        animator.SetTrigger("Primary");
    }

    public void onSecondary()
    {
        animator.SetTrigger("Secondary");
    }

    public void onReleaseAmmo()
    {
        ammoSpawner.enabled = true;
        ammoSpawner.once = true;
    }
}
