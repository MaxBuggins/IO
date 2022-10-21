using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
using Steamworks;
#endif
public class AchiementTrigger : MonoBehaviour
{
    public string achivementString = "ACH_FIND_CEREALS";

    public bool requireFinishedRaceAlive = false;

    void Start()
    {
        if (SteamManager.Initialized == false)
            Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
            if (player.isLocalPlayer)
            {
                if (requireFinishedRaceAlive == true && player.finishedRaceAlive == false)
                    return;

                SteamUserStats.SetAchievement(achivementString);
                SteamUserStats.StoreStats();
            }
    }
}
