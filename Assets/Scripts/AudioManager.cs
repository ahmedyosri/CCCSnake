using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioSource> audioObjects;
    
    public void Play(Track trackToPlay, bool forcePlay = false)
    {
        var trackId = (int)trackToPlay;

        if (audioObjects[trackId].isPlaying && !forcePlay)
        {
            return;
        }
        audioObjects[trackId].Play();
    }

    public void Stop(Track trackToStop)
    {
        var trackId = (int)trackToStop;

        if (!audioObjects[trackId].isPlaying)
        {
            return;
        }

        audioObjects[trackId].Stop();
    }

    public void SetVolume(Track trackToSetVolume, float newVolume = 0.5f)
    {
        var trackId = (int)trackToSetVolume;

        if (!audioObjects[trackId].isPlaying)
        {
            return;
        }

        audioObjects[trackId].volume = newVolume;
    }
}

public enum Track
{
    BackgroundMusic,
    Roll,
    Pickup
}