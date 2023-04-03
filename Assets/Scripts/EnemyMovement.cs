using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;
    [SerializeField] private Transform movePosTransform;
   
    private NavMeshAgent agent;
    [SerializeField] private float margin = 5f;
    [SerializeField] private float distenceToTarget;
    [SerializeField] bool destination = false;

    EnemyDetection enemyDetection;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        agent = GetComponent<NavMeshAgent>();

        walkingToPos = endPos;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distenceToTarget = Vector3.Distance(transform.position, walkingToPos);

        if (enemyDetection.detected == true)
        {
            agent.destination = movePosTransform.position;

        }
        else
        {
            agent.destination = walkingToPos;


            if (Vector3.Distance(transform.position, walkingToPos) < margin)
            {
                if (destination)
                {
                    walkingToPos = startingPos;
                    destination = false;
                }
                else
                {
                    walkingToPos = endPos;
                    destination = true;
                }
            }
        }
    }
}
