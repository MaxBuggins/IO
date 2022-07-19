using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class UI_Effect : MonoBehaviour
{
    public float moveScale = 1;

    public bool squirming = false;
    public float squirmSize = 4;
    private float timeSinceSquirm;
    public float squirmFrequency = 1;

    private Vector2 orginPos;

    private void Start()
    {
        orginPos = transform.position;
    }

    private void Update()
    {

    }

    void StartSquirm()
    {
        if (squirming == false)
            return;

        Vector2 moveAmount = Random.insideUnitSphere * squirmSize;
        moveAmount = (moveAmount / moveScale) * moveScale;

        transform.position = orginPos + moveAmount;

        Invoke(nameof(StartSquirm), squirmFrequency);
    }
    public void EnableSquirm()
    {
        squirming = true;
        StartSquirm();
    }
    public void DisableSquirm()
    {
        squirming = false;
    }
}
