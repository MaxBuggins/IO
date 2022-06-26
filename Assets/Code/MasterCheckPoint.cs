using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MasterCheckPoint : NetworkBehaviour
{
    [SyncVar(hook = nameof(CompleteCheckPoints))]
    public bool finished = false; //Player has finshed placing check points or not

    [SyncVar(hook = nameof(ClientActivateCheckPoints))]
    public bool active = true;

    public List<CheckPoint> checkPoints = new List<CheckPoint>();


    public string raceName = "The Nameless Race";
    [SyncVar] public Color32 colour;

    public GameObject checkPointPrefab;

    public override void OnStartClient()
    {
        if (!isClientOnly) //no HOST
            return;

        CmdGetRaces(); //TEMP

        RefreshCheckPoints();
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        RefreshCheckPoints();
        base.OnStartServer();
    }

    [Command]
    public void CmdGetRaces()
    {
        //TEMP
    }


    public void RefreshCheckPoints()
    {
        int i;
        for(i = 0; i < checkPoints.Count; i++)
        {
            checkPoints[i].checkPointIndex = i;
            checkPoints[i].SetCheckPoint();
        }

        CompleteCheckPoints(false, finished); //stupid bools needed because of hook
    }


    void CompleteCheckPoints(bool oldBool, bool newBool)
    {
        if (newBool)
        {
            //checkPoints[checkPoints.Count - 1].finish = true; //does the trick
            checkPoints[checkPoints.Count - 1].SetAsFinish(); //also does the trick
            checkPoints[0].SetVisabilty(0.7f);
        }
    }


    [TargetRpc]
    public void TRpcStartRace(NetworkConnection target)
    {
        Player.localInstance.currentRace = this;

        foreach (CheckPoint checkPoint in checkPoints)
            checkPoint.SetVisabilty(0.7f);

        //Color textColor = masterCheckPoint.colour;
        //textColor.a = 0.65f;

        //UI_Main.instance.CreateAlert("|| Start Race ||", 80, textColor);
    }

    public void EndRace(Player player, bool finish)
    {
        if (isServer)
        {
            if (finish)
                player.bestTime = (float)(player.checkPointTimes[player.checkPointTimes.Count - 1] - player.checkPointTimes[0]);
        }

        player.currentRace = null;

        foreach (CheckPoint checkPoint in checkPoints)
            checkPoint.SetVisabilty(0.12f);

        checkPoints[0].SetVisabilty(0.7f);
    }


    [Server]
    public void ServerCreateCheckPoint(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        //Fish
        GameObject checkPointObj = Instantiate(checkPointPrefab, position, rotation, transform);
        checkPointObj.transform.localScale = scale;

        CheckPoint checkPoint = checkPointObj.GetComponent<CheckPoint>();

        checkPoints.Add(checkPoint);

        checkPoint.masterCheckPoint = this;
        checkPoint.checkPointIndex = checkPoints.IndexOf(checkPoint);

        //rotations.Add(rotation.eulerAngles);
        //positions.Add(position);

        RpcCreateCheckPoint(position, rotation, scale);

        checkPoint.SetCheckPoint();
    }

    [ClientRpc]
    private void RpcCreateCheckPoint(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (!isClientOnly)
            return;

        GameObject checkPointObj = Instantiate(checkPointPrefab, position, rotation, transform);
        checkPointObj.transform.localScale = scale;

        CheckPoint checkPoint = checkPointObj.GetComponent<CheckPoint>();

        checkPoints.Add(checkPoint);

        checkPoint.masterCheckPoint = this;
        checkPoint.checkPointIndex = checkPoints.IndexOf(checkPoint);

        checkPoint.SetCheckPoint();
    }

    //Temp Code is bellow vvv

    [Server]
    public void ServerActivateCheckPoints(bool activate)
    {
        active = activate;


        foreach (CheckPoint checkPoint in checkPoints)
        {
            checkPoint.gameObject.SetActive(active);
        }

        RefreshCheckPoints();
    }

    public void ClientActivateCheckPoints(bool oldBool, bool newBool)
    {

        foreach (CheckPoint checkPoint in checkPoints)
        {
            checkPoint.gameObject.SetActive(newBool);
            checkPoint.TurnOnCheckPoint(newBool);
        }

        RefreshCheckPoints();
    }
}
