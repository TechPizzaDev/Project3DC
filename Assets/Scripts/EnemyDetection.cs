using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private float viewRadius; 
    [SerializeField] private float viewAngle;
    public bool detected;
    
    //Player layer
    public LayerMask targetPlayer; 
    public LayerMask obstacleMask;

    public GameObject player;
    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }
    
    void Update()
    {
        //Responsible for angle of player & enemy
        Vector3 playerTarget = (player.transform.position - transform.position).normalized;
        
        //Checks if enemies forward position & playertarget is less than the viewing angle / 2. Divide by 2 to create 45 degrees towards
        //both left and right.
        if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2 || enemyHealth.health < enemyHealth.maxHealth) 
        {
            float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if(distanceToTarget <= viewRadius)
            {
                //If there is no obstacle in the way = enemy has detected the player. 
                if(!Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacleMask))
                {
                    detected = true;
                    Debug.Log("You have been spotted.");
                }
                else
                {
                    detected = false;
                }
            }
            else
            {
                detected = false;
            }
        }
        else
        {
            detected = false;
        }
    }
}
