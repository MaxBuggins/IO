using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreTrigger : NetworkBehaviour
{
    public int addScore = 0;

    void Awake()
    {
        if (isClientOnly)
            Destroy(this);
    }


    [Server]
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) //player only, other bozos get out
            return;

        player.bonusScore += addScore;
    }
}
