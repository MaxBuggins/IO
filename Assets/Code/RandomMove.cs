using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class RandomMove : MonoBehaviour
{
    private Vector3 moveAmount;
    public Vector3 randomAmount;
    public bool relativeMove = true;

    public float duration;

    public AnimationCurve animationCurve;

    private Vector3 orginPos;

    void Start()
    {
        orginPos = transform.position;
        CompleteMove();
    }

    void CompleteMove()
    {
        moveAmount = new Vector3(Random.Range(-randomAmount.x, randomAmount.x),
            Random.Range(-randomAmount.y, randomAmount.y),
            Random.Range(-randomAmount.z, randomAmount.z));

        if (relativeMove)
            moveAmount += transform.position;
        else
            moveAmount += orginPos;

        Tween.LocalPosition(transform, moveAmount, duration, 0, animationCurve, Tween.LoopType.PingPong, completeCallback: CompleteMove);
    }
}
