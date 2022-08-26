using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;
using Pixelplacement.TweenSystem;

[RequireComponent(typeof(AudioSource))]
public class NetworkSpeaker : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip[] audioClips;
    public float delayOffset = 0;
    public float frequency = 3;

    private double lastPlay = 0;

    [Header("Animation")]
    public Vector3 maxScale;
    public float duration = 0.8f;
    public AnimationCurve animationCurve;

    private TweenBase tween;


    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastPlay -= delayOffset;

        tween = Tween.LocalScale(transform, maxScale, duration, 0, animationCurve, Tween.LoopType.PingPong);
    }

    void Update()
    {
        if (audioSource.isPlaying == false)
        {
            tween.Stop();
        }

        if (NetworkTime.time > lastPlay + frequency) 
        {
            float value = MyNetworkManager.singleton.GetNetworkRandomValue();

            audioSource.PlayOneShot(audioClips[(int)(value * audioClips.Length)]);

            lastPlay = NetworkTime.time;

            tween.Start();
        }
    }
}