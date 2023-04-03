using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject enemy;
    public int numEnemies = 3;
    public GameObject[] allEnemies;
    
    //Space that the enemies can move within 
    public Vector3 positionalLimit = new Vector3 (5,5,5); 

    void Start()
    {
        allEnemies = new GameObject[numEnemies];
        for(int i = 0; i < numEnemies; i++)
        {
            //Position of the enemies are relative to where the flock manager is.
            //Random ranges around the positional limit when spawning the enemies.
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-positionalLimit.x, positionalLimit.x),
                                                                Random.Range(-positionalLimit.y, positionalLimit.y),
                                                                Random.Range(-positionalLimit.z, positionalLimit.z));
           
            allEnemies[i] = Instantiate(enemy, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
