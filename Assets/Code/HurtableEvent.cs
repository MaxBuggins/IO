using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;

public class HurtableEvent : Hurtable
{
    [Header("Event")]
    [SerializeField] private UnityEvent onHurtEvent;
    [SerializeField] private bool serverOnlyHurtEvent;

    [SerializeField] private UnityEvent onDeathEvent;
    [SerializeField] private bool serverOnlyDeathEvent;

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

        if (serverOnlyHurtEvent && isServer == false)
            return;

        onHurtEvent.Invoke();
    }

    public override void OnDeath()
    {
        //not Killable
        health = maxHealth;

        Tween.LocalScale(transform, endScale, duration, 0, hurtAnimationCurve, completeCallback: ResetScale);

        if (serverOnlyDeathEvent && isServer == false)
            return;

        onDeathEvent.Invoke();
    }

    void ResetScale()
    {
        Tween.LocalScale(transform, startScale, duration / 2, 0, hurtAnimationCurve);
    }
}
