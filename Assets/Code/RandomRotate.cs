using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class RandomRotate : MonoBehaviour
{
    private Vector3 roatateAmount;
    public Vector3 randomAmount;
    public bool relativeRotation = true;

    public float duration;

    public AnimationCurve animationCurve;

    void Start()
    {
        CompleteRotate();
    }

    void CompleteRotate()
    {
        roatateAmount.x = Random.Range(-randomAmount.x, randomAmount.x);
        roatateAmount.y = Random.Range(-randomAmount.y, randomAmount.y);
        roatateAmount.z = Random.Range(-randomAmount.z, randomAmount.z);

        if (relativeRotation)
            Tween.Rotate(transform, roatateAmount, Space.Self, duration, 0, animationCurve, completeCallback: CompleteRotate);
        else
            Tween.Rotation(transform, roatateAmount, duration, 0, animationCurve, completeCallback: CompleteRotate);
    }

}
