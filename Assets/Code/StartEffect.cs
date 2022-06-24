using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class StartEffect : MonoBehaviour
{
    public Vector3 endScale = Vector3.one;
    public float duration;
    public AnimationCurve animationCurve;

    void Start()
    {
        Tween.LocalScale(transform, endScale, duration, 0, animationCurve);
        Destroy(this);
    }

}
