using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    //list of bullets
    private List<GameObject> bullets = new List<GameObject>();

    private void Update()
    {
        RemoveDestroyed();
    }

    //remove destroyed bullets from list
    private void RemoveDestroyed()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i] == null)
            {
                bullets.RemoveAt(i);
            }
        }
    }

    public void AddBullet(GameObject bullet)
    {
        bullets.Add(bullet);
    }
    
}
