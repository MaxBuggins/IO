using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirectionalSprite))] //fancy still

public class PlayerCorpse : MonoBehaviour
{
    [SerializeField] private float tryDisableDelay = 4;
    [SerializeField] private float maxDisableVelocity = 1;

    public Sprite[] deathSprites;
    public float[] frameDelays;


    private int frameIndex = 0;

    private DirectionalSprite directionalSprite;
    private Rigidbody rb;

    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();
        rb = GetComponentInParent<Rigidbody>();

        Invoke(nameof(NextFrame), frameDelays[0]);
        Invoke(nameof(TryDisable), tryDisableDelay);

    }

    void NextFrame()
    {
        directionalSprite.render.sprite = deathSprites[frameIndex];
        frameIndex += 1;

        if (frameIndex >= deathSprites.Length)
            return;

        Invoke(nameof(NextFrame), frameDelays[frameIndex]);

    }

    void TryDisable()
    {
        if (rb.velocity.magnitude > maxDisableVelocity)
        {
            Invoke(nameof(TryDisable), tryDisableDelay);
            return;
        }

        transform.parent = null;
        Destroy(rb.gameObject);   
    }
}
