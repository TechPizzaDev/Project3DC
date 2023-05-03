using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;
    public GameObject boomBug;
    public GameObject shellShock;
    public int numEnemies = 3;
    public GameObject[] allEnemies;
    //Space that the enemies can move within 
    public Vector3 positionalLimit = new Vector3(5, 5, 5);

    [Range(1.0f, 10.0f)]
    public float neighbourDistance; 
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;


    void Start()
    {
        FM = this;

        SpawnEnemies(boomBug);
        SpawnEnemies(shellShock);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnEnemies(GameObject enemyType)
    {
        allEnemies = new GameObject[numEnemies];
        for (int i = 0; i < numEnemies; i++)
        {
            //Position of the enemies are relative to where the flock manager is.
            //Random ranges around the positional limit when spawning the enemies.
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-positionalLimit.x, positionalLimit.x),
                                                                Random.Range(-positionalLimit.y, positionalLimit.y),
                                                                Random.Range(-positionalLimit.z, positionalLimit.z));

            allEnemies[i] = Instantiate(enemyType, pos, Quaternion.identity);
        }
    }
}
