using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Animator))] //fancy
public class PlayerAnimator : MonoBehaviour
{
    private float distanceSinceStep = 0;
    public float stepDistance = 1f;

    public float movementMultiplyer = 10;


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private static readonly float maxCastDistance = 0.8f;

    [SerializeField] private SurfaceMaterialData _surfaceData = null;


    private Vector3 lastPos;

    private Animator animator;
    private Player player;
    private AudioSource audioSource;


    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
        audioSource = GetComponentInParent<AudioSource>();
    }


    void FixedUpdate()
    {
        animator.SetBool("Grounded", player.playerMovement.onGround);

        float moveMagnatuide = Vector3.Distance(player.transform.position, lastPos);
        distanceSinceStep += moveMagnatuide;

        moveMagnatuide *= movementMultiplyer;
        moveMagnatuide = Mathf.Round(moveMagnatuide * 10f) / 10f;

        animator.SetFloat("move", moveMagnatuide);

        print(moveMagnatuide);

        if (distanceSinceStep > stepDistance)
        {
            OnFootstepTaken();
            distanceSinceStep = 0;
        }

        lastPos = player.transform.position;
    }


    protected void OnFootstepTaken()
    {
        Ray ray = new Ray(transform.position + (Vector3.up / 2), Vector3.down);
        SurfaceMaterial castResult = Cast(ray);

        audioSource.pitch = Random.Range(0.5f, 1.5f);
        audioSource.volume = Random.Range(0.5f, 1f);

        if (castResult != null)
            audioSource.PlayOneShot(castResult.clips[Random.Range(0, castResult.clips.Length)]); // pick one at random
    }


    public SurfaceMaterial Cast(Ray ray)
    {
        if (_surfaceData != null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxCastDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                Terrain terrain = hitInfo.transform.GetComponent<Terrain>();
                if (terrain != null)
                {
                    return CastTerrain(terrain, hitInfo.point);
                }

                Renderer renderer = hitInfo.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    if (material != null)
                    {
                        SurfaceMaterial surfaceMaterial = _surfaceData.FindSurfaceMaterial(material);
                        return surfaceMaterial;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("SurfaceData not assigned (Dummy Stupid)", this);
        }
        return null;
    }

    private SurfaceMaterial CastTerrain(Terrain terrain, Vector3 position)
    {
        TerrainData data = terrain.terrainData;

        float[,,] splatMapData = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight); // In our dev code, I cache this result, not having to fetch it every step.
        int splatTextureCount = splatMapData.Length / (data.alphamapWidth * data.alphamapHeight);

        Vector3 localHit = terrain.transform.InverseTransformPoint(position);
        Vector3 splatPosition = new Vector3(
            (localHit.x / data.size.x) * data.alphamapWidth,
            0,
            (localHit.z / data.size.z) * data.alphamapHeight);

        //Get the most opaque splat
        float maxOpaque = 0f;
        int index = -1;
        for (int i = 0; i < splatTextureCount; i++)
        {
            float opacity = splatMapData[(int)splatPosition.z, (int)splatPosition.x, i];
            if (opacity > maxOpaque)
            {
                maxOpaque = opacity;
                index = i;
            }
        }

        //Fetch
        TerrainLayer layer = data.terrainLayers[index];
        SurfaceMaterial surfaceMaterial = _surfaceData.FindSurfaceMaterial(layer);
        return surfaceMaterial;
    }
}

