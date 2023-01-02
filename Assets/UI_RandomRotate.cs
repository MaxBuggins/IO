using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class UI_RandomRotate : MonoBehaviour
{
    public Vector2 rotateRange;
    public float rotateDuration = 0;

    public AnimationCurve rotateAnimationCurve;

    void OnEnable()
    {
        Quaternion endRot = Quaternion.Euler(0, 0, Random.Range(rotateRange.x, rotateRange.y));

        if (rotateDuration > 0)
        {
            transform.eulerAngles = new Vector3(90, 0, 0);
            Tween.LocalRotation(transform, endRot, rotateDuration, 0, rotateAnimationCurve);
        }
        else
            transform.rotation = endRot;

    }
}
