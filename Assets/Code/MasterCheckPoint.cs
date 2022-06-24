using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MasterCheckPoint : NetworkBehaviour
{
    [SyncVar(hook = nameof(CompleteCheckPoints))]
    public bool finished = false; //Player has finshed placing check points or not

    public List<CheckPoint> checkPoints = new List<CheckPoint>();

    public string raceName = "The Nameless Race";
    [SyncVar] public Color32 colour;

    public readonly SyncList<Vector3> positions = new SyncList<Vector3>();
    public readonly SyncList<Vector3> rotations = new SyncList<Vector3>();

    public GameObject checkPointPrefab;

    public void Start()
    {
        if(checkPoints.Count > 1) //In case checkpoints are preset in scene
            RefreshCheckPoints();
    }


    public void RefreshCheckPoints()
    {
        int i;
        for(i = 0; i < checkPoints.Count; i++)
        {
            checkPoints[i].checkPointIndex = i;
            checkPoints[i].SetCheckPoint();
        }

        CompleteCheckPoints(false, true); //stupid bools needed because of hook
    }


    void CompleteCheckPoints(bool oldBool, bool newBool)
    {
        //checkPoints[checkPoints.Count - 1].finish = true; //does the trick
        checkPoints[checkPoints.Count - 1].SetAsFinish(); //also does the trick
        checkPoints[0].SetVisabilty(0.7f);
    }

    [Server]
    public void ServerCreateCheckPoint(Vector3 position, Quaternion rotation)
    {
        //1 Line = 3 Lines (Code Has Momented)
        CheckPoint checkPoint =
            Instantiate(checkPointPrefab, position, rotation, transform)
            .GetComponent<CheckPoint>();

        checkPoints.Add(checkPoint);
        positions.Add(position);
        rotations.Add(rotation.eulerAngles);

        checkPoint.masterCheckPoint = this;
        checkPoint.checkPointIndex = checkPoints.IndexOf(checkPoint);

        RpcCreateCheckPoint(position, rotation);

        checkPoint.SetCheckPoint();
    }

    [ClientRpc]
    private void RpcCreateCheckPoint(Vector3 position, Quaternion rotation)
    {
        if (!isClientOnly)
            return;

        CheckPoint checkPoint =
            Instantiate(checkPointPrefab, position, rotation, transform)
            .GetComponent<CheckPoint>();

        checkPoints.Add(checkPoint);
        checkPoint.masterCheckPoint = this;
        checkPoint.checkPointIndex = checkPoints.IndexOf(checkPoint);

        checkPoint.SetCheckPoint();
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

    public void EndRace(Player player)
    {
        player.currentRace = null;

        foreach (CheckPoint checkPoint in checkPoints)
            checkPoint.SetVisabilty(0.12f);

        checkPoints[0].SetVisabilty(0.7f);
    }
}
