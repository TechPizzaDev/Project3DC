using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int maxBounceCount;
    public int MaxBounceCount
    {
        get { return maxBounceCount; }
        set { maxBounceCount = value; }
    }
    

    [SerializeField]
    private int bounceCount;

    private void OnCollisionEnter(Collision other)
    {
        bounceCount++;
        
        if (bounceCount >= maxBounceCount)
        {
            DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
