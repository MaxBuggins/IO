using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ConstantMove : MonoBehaviour
{
    public Vector3 moveAmount;
    public float duration;
    public AnimationCurve animationCurve;
    public Tween.LoopType loopType;

    void Start()
    {
        Tween.LocalPosition(transform, moveAmount, duration, 0, animationCurve, loop: loopType);
    }

}
