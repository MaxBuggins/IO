using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;

public class HurtableEvent : Hurtable
{
    [Header("Event")]
    [SerializeField] private UnityEvent onHurtEvent;
    [SerializeField] private bool serverOnlyEvent;

    [Header("Scale")]
    public Vector3 endScale;
    private Vector3 startScale;
    public float duration = 0.15f;
    public AnimationCurve hurtAnimationCurve;

    private void Start()
    {
        startScale = transform.localScale;
    }

    public override void OnHurt(int damage)
    {
        Tween.LocalScale(transform, endScale, duration, 0, hurtAnimationCurve, completeCallback: ResetScale);

        onHurtEvent.Invoke();
    }

    void ResetScale()
    {
        Tween.LocalScale(transform, startScale, duration / 2, 0, hurtAnimationCurve);
    }

    public override void OnDeath()
    {
        //not Killable
        health = maxHealth;
        OnHurt(10);
    }
}
