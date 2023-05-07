using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
public class AudioConfig : ScriptableObject, System.ICloneable
{
    [Range(0, 1f)]
    public float volume = 1f;
    public AudioClip[] fireClips;
    public AudioClip emptyClip;
    public AudioClip reloadClip;
    public AudioClip lastBulletClip;
    
    public void PlayShootingClip(AudioSource audioSource, bool isLastBullet = false)
    {
        if (isLastBullet && lastBulletClip != null)
        {
            audioSource.PlayOneShot(lastBulletClip, volume);
        }
        else
        {
            audioSource.PlayOneShot(fireClips[Random.Range(0, fireClips.Length)], volume);
        }
    }

    public void PlayOutOfAmmoClip(AudioSource audioSource)
    {
        if (reloadClip != null)
        {
            audioSource.PlayOneShot(emptyClip, volume);
        }
    }

    public void PlayReloadClip(AudioSource audioSource)
    {
        if (reloadClip != null)
        {
            audioSource.PlayOneShot(reloadClip, volume);
        }
    }

    public object Clone()
    {
        AudioConfig config = CreateInstance<AudioConfig>();

        Utilities.CopyValues(this, config);

        return config;
    }
}
