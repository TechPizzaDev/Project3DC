using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyRules();
    }

    void ApplyRules()
    {
        GameObject[] gameObjects = FlockManager.FM.allEnemies;

        Vector3 vectorCenter = Vector3.zero;
        Vector3 vectorToAvoid = Vector3.zero;
        float groupSpeed = 0.01f;
        float neighbourDistance;
        int groupSize = 0; 

        foreach(GameObject gameObject in gameObjects)
        {
            if(gameObject != this.gameObject)
            {
                neighbourDistance = Vector3.Distance(gameObject.transform.position, this.transform.position);  
                if(neighbourDistance <= FlockManager.FM.neighbourDistance)
                {
                    vectorCenter += gameObject.transform.position;
                    groupSize++;

                    if(neighbourDistance < 1.0f)
                    {
                        vectorToAvoid = vectorToAvoid + (this.transform.position - gameObject.transform.position);

                    }

                    Flock anotherFlock = gameObject.GetComponent< Flock>();
                    groupSpeed = groupSpeed + anotherFlock.speed;
                }
            }
        }
    }
}
