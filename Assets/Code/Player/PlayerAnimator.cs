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


    private float animationLockedTill;
    private float animationCrossFade = 0;

    #region Cached Properties

    private int _currentState;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int fall = Animator.StringToHash("Fall");
    private static readonly int land = Animator.StringToHash("Land");
    private static readonly int roll = Animator.StringToHash("Roll");
    private static readonly int crouch = Animator.StringToHash("Crouch");
    private static readonly int crouchWalk = Animator.StringToHash("Crouch Walk");
    private static readonly int shout = Animator.StringToHash("Shout");
    private static readonly int primaryAttack = Animator.StringToHash("Primary Attack");
    private static readonly int secondaryAttack = Animator.StringToHash("Secondary Attack");
    #endregion

    #region FaceMaterials
    [Header("Face Materials")]
    [SerializeField] private Material smile;
    [SerializeField] private Material smileTeeth;
    [SerializeField] private Material mouth;
    [SerializeField] private Material mouthOpen;
    #endregion

    #region AudioSorces
    [Header("Audio Sources")]
    public AudioSource feetAudioSource;
    public AudioSource mouthAudioSource;

    private float clipSampleRate;
    private int sampleDataLength = 1024;

    private float timeSinceLateUpdate = 0;
    private float lateUpdateRate = 0.3f;

    #endregion

    private Vector3 lastPos;
    private Vector3 lastMoveVector;
    private Vector3 moveVector;

    private Animator animator;
    private Player player;

    [HideInInspector] public SkinnedMeshRenderer playerMeshRenderer;

    public GameObject LandObject;


    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
        playerMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }


    private void FixedUpdate()
    {
        moveVector = player.transform.position - lastPos;
        moveVector.y = 0;

        moveVector *= movementMultiplyer;

        lastPos = player.transform.position;
        lastMoveVector = moveVector;
    }

    public void ChangeFace(float loudness)
    {
        Material[] materials = playerMeshRenderer.materials;
        if (loudness > 0.01f)
        {
            if (loudness > 0.06f)
                materials[1] = mouthOpen;

            else if (loudness > 0.035f)
                materials[1] = mouth;

            else
                materials[1] = smileTeeth;       
        }

        else
        {
            materials[1] = smile;
            animator.SetBool("Shouting", false);
        }
        playerMeshRenderer.materials = materials;
    }

    void Update()
    {
        float moveMag = Mathf.Clamp(moveVector.magnitude, 0.2f, 20);
        animator.SetFloat("move", moveMag);
        distanceSinceStep += moveVector.magnitude;

        if (distanceSinceStep > stepDistance && player.timeSinceGrounded <= 0)
        {
            OnFootstepTaken();
            distanceSinceStep = 0;
        }

        var state = GetState();
        //var armState = GetArmState();


        if (state == _currentState) 
            return;
        

        animator.CrossFade(state, animationCrossFade, 0);
        animator.CrossFade(state, animationCrossFade, 1);
        //transform.localPosition = Vector3.zero; //reset root
        _currentState = state;
    }

    private void LateUpdate()
    {
        timeSinceLateUpdate += Time.deltaTime;
        if (timeSinceLateUpdate >= lateUpdateRate)
        {
            timeSinceLateUpdate = 0f;

            float[] clipSampleData = new float[sampleDataLength];
            mouthAudioSource.GetOutputData(clipSampleData, mouthAudioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            float clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for

            print(clipLoudness);
            ChangeFace(clipLoudness);
        }
    }

    private int GetState()
    {
        animationCrossFade = 0;

        if (Time.time < animationLockedTill) return _currentState;

        // Crouch
        if (player.crouching)
        {
            if (player.timeSinceGrounded > 0.4)
            {
                return fall;
            }

            if (moveVector.magnitude > 0.2f)
            {
                if (Mathf.Abs(moveVector.y) > 1f || moveVector.magnitude > 4)
                    return fall;

                return crouchWalk;
            }

            return LockState(crouch, 0.1f, 0.1f);
        }


        if(player.timeSinceGrounded > 0.4)
        {
            return fall;
        }

        if(moveVector.magnitude > 0.2f)
        {
            if (Mathf.Abs(moveVector.y) > 1f || moveVector.magnitude > 6)
                return fall;

            return run;           
        }


        return LockState(idle, 0.15f, 0.15f);

        int LockState(int state, float time, float fade)
        {
            animationLockedTill = Time.time + time;
            animationCrossFade = fade;
            return state;
        }
    }


    protected void OnFootstepTaken()
    {
        Ray ray = new Ray(transform.position + (Vector3.up / 2), Vector3.down);
        SurfaceMaterial castResult = Cast(ray);

        feetAudioSource.pitch = Random.Range(0.5f, 1.5f);
        feetAudioSource.volume = Random.Range(0.3f, 0.6f);

        if (castResult != null)
            feetAudioSource.PlayOneShot(castResult.clips[Random.Range(0, castResult.clips.Length)]); // pick one at random
    }

    public void TriggerPrimaryAttack()
    {
        animator.CrossFade(primaryAttack, 0, 1);
    }

    public void TriggerSecondaryAttack()
    {
        animator.CrossFade(secondaryAttack, 0, 1);
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


    public void OnHardLanding()
    {
        if (_currentState == land || _currentState == roll)
        {
            return;
        }

        int state = land;

        if(moveVector.magnitude > 0.5f)
        {
            state = roll;
        }

        animator.CrossFade(state, 0.1f, 0);
        animationLockedTill = Time.time + 1f;
        _currentState = state;


        Instantiate(LandObject, transform.position, transform.rotation, null);
    }


    public void PlayCallout(int set, int index)
    {
        animator.SetBool("Shouting", true);
        AudioClip[] playerCallout = player.characteristicsObject.playerCallouts[set].clips;
        mouthAudioSource.PlayOneShot(playerCallout[index]);
    }
}



