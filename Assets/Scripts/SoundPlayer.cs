using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    public AudioSource src;
    public AudioClip sfx1, sfx2, sfx3;

    public void Explosion()
    {
        src.clip = sfx1;
        src.Play();
    }
}
