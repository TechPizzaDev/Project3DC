using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        audioSource.PlayOneShot(explosionSfx, volume);
    }
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
