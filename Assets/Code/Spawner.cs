using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool once;

    private float timeSinceSpawn = 0;
    public float delay;

    public GameObject toSpawn;
    public int spawnAmount = 1;
    public float spawnRadius = 0;


    void Start()
    {
        if (delay == 0)
        {
            enabled = false;
        }


    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if (timeSinceSpawn > delay)
        {
            timeSinceSpawn = 0;
            spawn();

            if (once)
                enabled = false;
        }
    }

    public void spawn()
    {
        for (int a = 0; a < spawnAmount; a++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            Instantiate(toSpawn, spawnPos, transform.rotation, null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 1, 0.4f);

        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}
