using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ConstantMove : MonoBehaviour
{
    public Vector3 moveAmount;

    public float duration;
    public float delay = 0;

    public AnimationCurve animationCurve;

    public Tween.LoopType loopType;

    void Start()
    {
        Tween.LocalPosition(transform, moveAmount, duration, delay, animationCurve, loop: loopType);
    }

}
