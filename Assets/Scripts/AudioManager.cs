using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sounds in the Main scene
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Set from the inspector, this list should contain all the <see cref="AudioSource"/> components running across the scene, also a corresponding entries in <see cref="Track"/> should be set to access these audios
    /// </summary>
    [SerializeField]
    private List<AudioSource> audioObjects;
    
    /// <summary>
    /// Plays a soundtrack (AudioSource)
    /// </summary>
    /// <param name="trackToPlay">The specefic soundtrack to play, you can add more tracks by adding to AudioManager->Audio Objects list from the inspector and a corresponding track name in <see cref="Track"/></param>
    /// <param name="forcePlay">Ignores checking if the sound is already running, plays it from the beginning</param>
    public void Play(Track trackToPlay, bool forcePlay = false)
    {
        var trackId = (int)trackToPlay;

        if (audioObjects[trackId].isPlaying && !forcePlay)
        {
            return;
        }
        audioObjects[trackId].Play();
    }

    /// <summary>
    /// Stops a currently running soundtrack
    /// </summary>
    /// <param name="trackToStop">The specefic soundtrack to stop, you can add more tracks by adding to AudioManager->Audio Objects list from the inspector and a corresponding track name in <see cref="Track"/></param>
    public void Stop(Track trackToStop)
    {
        var trackId = (int)trackToStop;

        if (!audioObjects[trackId].isPlaying)
        {
            return;
        }

        audioObjects[trackId].Stop();
    }

    /// <summary>
    /// Sets the volume of a currently running soundtrack
    /// </summary>
    /// <param name="trackToSetVolume">The specefic soundtrack to set its volume, you can add more tracks by adding to AudioManager->Audio Objects list from the inspector and a corresponding track name in <see cref="Track"/></param>
    /// <param name="newVolume">The new volume level from 0 to 1</param>
    public void SetVolume(Track trackToSetVolume, float newVolume = 0.5f)
    {
        var trackId = (int)trackToSetVolume;

        if (!audioObjects[trackId].isPlaying)
        {
            return;
        }

        audioObjects[trackId].volume = Mathf.Clamp(newVolume, 0, 1);
    }
}

/// <summary>
/// Represents tracks attached to the <see cref="AudioManager.audioObjects"/> from the inspector
/// </summary>
public enum Track
{
    BackgroundMusic,
    Roll,
    Pickup
}