using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [Header("Requirements")]
    public bool localPlayerOnly;

    [SerializeField]
    private UnityEvent onTriggerEnterEvent;

    [SerializeField]
    private UnityEvent onTriggerExitEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (localPlayerOnly && other.gameObject != Player.localInstance.gameObject)
            return;

        onTriggerEnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (localPlayerOnly && other.gameObject != Player.localInstance.gameObject)
            return;

        onTriggerExitEvent.Invoke();
    }
}
