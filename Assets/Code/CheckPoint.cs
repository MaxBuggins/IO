using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Pixelplacement;

public class CheckPoint : MonoBehaviour
{
    public float activeAnimationDuration = 0.5f;
    public AnimationCurve activeAnimationCurve;

    public int checkPointIndex = -1;

    public bool finish = false;

    public MasterCheckPoint masterCheckPoint;

    private Vector3 storedScale;

    [SerializeField] private TextMeshPro textDisplay;
    [SerializeField] private Renderer ringRender;
    [SerializeField] private Renderer centerRender;

    private void Start()
    {
        if (masterCheckPoint.active == false)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (checkPointIndex > 0)
            return;

        if (LevelManager.instance == null)
            return;

        string minutes = Mathf.Floor(LevelManager.instance.raceTimeRemaining / 60).ToString("00");
        string seconds = Mathf.Floor(LevelManager.instance.raceTimeRemaining % 60).ToString("00");

        textDisplay.text = "Expire\n" + minutes + ":" + seconds;

    }

    public void SetCheckPoint()
    {
        SetVisabilty(0.12f);
        SetColour(masterCheckPoint.colour);

        if (checkPointIndex == 0)
        {
            SetVisabilty(0.7f);
            textDisplay.text = "Start";
        }
        else
        {
            textDisplay.text = checkPointIndex.ToString();
        }
    }

    public void TurnOnCheckPoint()
    {
        Vector3 currentScale = transform.localScale;
        transform.localScale = Vector3.zero;

        Tween.LocalScale(transform, currentScale, activeAnimationDuration, 0, activeAnimationCurve);
    }

    public void TurnOffCheckPoint()
    {
        storedScale = transform.localScale;

        Tween.LocalScale(transform, Vector3.zero, activeAnimationDuration, 0, activeAnimationCurve, completeCallback: RevertScale);
    }

    public void RevertScale()
    {
        transform.localScale = storedScale;
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
                    masterCheckPoint.EndRace(player, true);
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
