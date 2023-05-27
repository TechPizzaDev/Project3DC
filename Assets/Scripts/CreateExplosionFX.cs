using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class creates an explosion visual effect and plays an explosion sound effect.
/// It's meant to be used for Boombugs explosion when it attacks the player.
/// It destroys the game object after a specified duration.
/// </summary>

public class CreateExplosionFX : MonoBehaviour
{
    float timer = 1.5f;

    [Range(0, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip explosionSfx;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        // Play the explosion sound effect once with the specified volume
        audioSource.PlayOneShot(explosionSfx, volume);
    }
    void Update()
    {
        timer -= Time.deltaTime; 

        if(timer <= 0) // If the timer reaches or goes below zero, destroy the game object
        {
            Destroy(gameObject);
        }
    }
}
