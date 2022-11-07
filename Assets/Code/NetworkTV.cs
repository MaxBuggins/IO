using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Video;
using System.IO;

[RequireComponent(typeof(VideoPlayer))] //fancy
public class NetworkTV : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnURLChanged))] public string URL;

    private VideoPlayer videoPlayer;

    public ulong videoDuration
    {
        get { return (ulong)(videoPlayer.frameCount / videoPlayer.frameRate); }
    }

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (isServer)
        {
            string videoLinksFile = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "VideoLinks.txt"));
            string[] videoLinks = videoLinksFile.Split('~');

            URL = videoLinks[Random.Range(0, videoLinks.Length)];

        }

        else
            OnURLChanged(URL, URL);
    }

    public void OnURLChanged(string oldURL, string newURL)
    {
        videoPlayer.url = newURL;
        videoPlayer.Play();
        videoPlayer.time = NetworkTime.time;
    }
}
