using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Animator))] //fancy
public class PlayerAnimator : MonoBehaviour
{
    public float movementMultiplyer = 10;

    private Vector3 lastPos;

    private Animator animator;
    private Player player;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }


    void FixedUpdate()
    {
        float moveMagnatuide = Vector3.Distance(player.transform.position, lastPos);

        moveMagnatuide *= movementMultiplyer;
        moveMagnatuide = Mathf.Round(moveMagnatuide * 10f) / 10f;

        animator.SetFloat("move", moveMagnatuide);

        print(moveMagnatuide);

        lastPos = player.transform.position;
    }
}
