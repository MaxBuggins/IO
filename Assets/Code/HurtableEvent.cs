using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;
using Mirror;

public class HurtableEvent : Hurtable
{
    enum EventEnviroment { global, serverOnly, clientOnly, localOnly }

    [Header("Hurt Event")]
    [SerializeField] private UnityEvent onHurtEvent;
    [SerializeField] private EventEnviroment hurtEventEnviroment;

    [Header("Death Event")]
    [SerializeField] private UnityEvent onDeathEvent;
    [SerializeField] private EventEnviroment deathEventEnviroment;

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

        if (CheckEventEnviroment(hurtEventEnviroment))
            onHurtEvent.Invoke();
    }

    public override void OnDeath()
    {
        Tween.LocalScale(transform, endScale, duration, 0, hurtAnimationCurve, completeCallback: ResetScale);

        if(CheckEventEnviroment(deathEventEnviroment))
            onDeathEvent.Invoke();
    }

    [ServerCallback]
    public override void ServerDeath()
    {
        health = maxHealth;
    }

    bool CheckEventEnviroment(EventEnviroment eventEnviroment)
    {
        switch (eventEnviroment)
        {
            case (EventEnviroment.clientOnly):
                {
                    if (!isClient)
                        return (false);
                    break;
                }

            case (EventEnviroment.serverOnly):
                {
                    if (!isServer)
                        return (false);
                    break;
                }

            case (EventEnviroment.localOnly):
                {
                    if (lastAttackerIdenity != Player.localInstance.netIdentity)
                        return(false);
                    break;
                }
        }

        return (true);
    }

    void ResetScale()
    {
        Tween.LocalScale(transform, startScale, duration / 2, 0, hurtAnimationCurve);
    }
}
