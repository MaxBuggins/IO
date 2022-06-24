using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CheckPoint : MonoBehaviour
{
    public int checkPointIndex = -1;

    public bool finish = false;

    public MasterCheckPoint masterCheckPoint;

    [SerializeField] private TextMeshPro textDisplay;
    [SerializeField] private Renderer ringRender;
    [SerializeField] private Renderer centerRender;


    private void Start()
    {
        SetVisabilty(0.12f);
        SetColour(masterCheckPoint.colour);
    }

    public void SetCheckPoint()
    {
        if (checkPointIndex == 0)
        {
            textDisplay.text = "Start";

        }
        else
        {
            textDisplay.text = checkPointIndex.ToString();
        }
    }
    
    public void SetAsFinish()
    {
        finish = true;

        textDisplay.text = "END";
        textDisplay.fontSize = 12;
    }
    

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (masterCheckPoint.finished == false)
            return;

        Player player = other.GetComponent<Player>();
        if (player == null) //player only, other bozos get out
            return;

        if (player.currentRace == masterCheckPoint)
        {
            if(player.checkPointTimes.Count == checkPointIndex)
            {
                if (checkPointIndex == masterCheckPoint.checkPoints.Count - 1)
                {
                    player.checkPointTimes.Add(NetworkTime.time);

                    //player.checkPointTimes.Count = -1;
                    masterCheckPoint.EndRace(player);
                    return;
                }

                //will auto send message on player via SyncList
                player.checkPointTimes.Add(NetworkTime.time);

            }
        }

        else if(checkPointIndex == 0)
        {
            //TRpcStartCheckPoint(player.connectionToClient);

            masterCheckPoint.TRpcStartRace(player.connectionToClient);
            player.currentRace = masterCheckPoint;

            player.checkPointTimes.Clear();
            
            player.checkPointTimes.Add(NetworkTime.time);
        }

    }

    
/*    [TargetRpc]
    public void TRpcStartCheckPoint(NetworkConnection target)
    {
        Player.localInstance.currentRace = masterCheckPoint;

        Color textColor = masterCheckPoint.colour;
        textColor.a = 0.65f;

        UI_Main.instance.CreateAlert("|| Start Race ||", 80, textColor);
    }*/

    public void SetVisabilty(float visability)
    {
        Color ringColour = ringRender.material.color;
        ringColour.a = visability;
        ringRender.material.color = ringColour;

        Color centerColour = centerRender.material.color;
        centerColour.a = visability;
        centerRender.material.color = centerColour;

        Color textColour = textDisplay.color;
        textColour.a = visability;
        textDisplay.color = textColour;
    }

    public void SetColour(Color32 colour)
    {
        Color _colour = colour;

        _colour.a = ringRender.material.color.a;
        ringRender.material.color = _colour;

        _colour.a = centerRender.material.color.a;
        centerRender.material.color = _colour;
    }
}
