using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class VendingMachine : Hurtable
{
    [Header("Scale")]
    private Vector3 startScale;
    public Vector3 endScale;
    public float duration = 0.15f;
    public AnimationCurve hitAnimationCurve;

    [Header("Spawn")]
    public Vector2 canSpawnCountRange = new Vector2(1, 3);
    public Vector2 spawnSize;

    public GameObject[] cans;

    private void Start()
    {
        startScale = transform.localScale;
        spawnSize /= 2;
    }

    public override void OnHurt(int damage)
    {
        Tween.LocalScale(transform, endScale, duration, 0, hitAnimationCurve, completeCallback: ResetScale);

        int count = (int)Random.Range(canSpawnCountRange.x, canSpawnCountRange.y);

        for (int i = 0; i < count; i++)
        {

            Vector3 spawnLocation = transform.position + transform.forward * 0.5f;
            spawnLocation += Vector3.up * Random.Range(-spawnSize.y, spawnSize.y);
            spawnLocation += Vector3.forward * Random.Range(-spawnSize.x, spawnSize.x);


            GameObject spawned = Instantiate(cans[Random.Range(0, cans.Length)], spawnLocation, Quaternion.identity, LevelManager.instance.rbsParentObject.transform);
        }
        //spawned.GetComponent<Rigidbody>().AddForce(transform.forward);
    }

    void ResetScale()
    {
        Tween.LocalScale(transform, startScale, duration / 2, 0, hitAnimationCurve);
    }

    public override void OnDeath()
    {
        //not Killable
        health = maxHealth;
        OnHurt(10);
    }

}
