using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public enum triggerObjectType { anything, player, localPlayer }

    [Header("Requirements")]
    public triggerObjectType triggerObject;

    [SerializeField]
    private UnityEvent onTriggerEnterEvent;

    [SerializeField]
    private UnityEvent onTriggerExitEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (PassRequirements(other) == true)
            onTriggerEnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if(PassRequirements(other) == true)
            onTriggerExitEvent.Invoke();
    }

    bool PassRequirements(Collider other)
    {
        switch (triggerObject)
        {
            case (triggerObjectType.player):
                {
                    if (other.GetComponent<Player>() == null)
                        return (false);

                    break;
                }

            case (triggerObjectType.localPlayer):
                {
                    if (other.gameObject != Player.localInstance.gameObject)
                        return (false);

                    break;
                }
        }

        return (true);
    }
}
