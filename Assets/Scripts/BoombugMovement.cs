using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoombugMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;
    [SerializeField] public Transform movePosTransform;
   
    public NavMeshAgent agent;
    [SerializeField] private float margin = 5f;
    [SerializeField] public float distanceToTarget;
    [SerializeField] private float stopChasingTimer = 0f;
    [SerializeField] private float stopChasingTimerReset = 5f;
    [SerializeField] bool reachedDestination = false;
    bool isChasing;

    EnemyDetection enemyDetection;
    BoombugExplode boombugExplode;
    float aggressiveSpeed = 3.0f;
    float regularSpeed = 1.0f;

    enum State
    {
        isPatrolling,
        isAggro,
        isExploding
    }
    State state;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        boombugExplode = GetComponent<BoombugExplode>();
        agent = GetComponent<NavMeshAgent>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case State.isPatrolling:
                {
                    distanceToTarget = Vector3.Distance(transform.position, walkingToPos);
                    agent.destination = walkingToPos;
                    agent.speed = regularSpeed;

                    if (Vector3.Distance(transform.position, walkingToPos) < margin)
                    {
                        if (reachedDestination)
                        {
                            walkingToPos = startingPos;
                            reachedDestination = false;
                        }
                        else
                        {
                            walkingToPos = endPos;
                            reachedDestination = true;
                        }
                    }
                    if (enemyDetection.detected)
                    {
                        state = State.isAggro;
                        stopChasingTimer = stopChasingTimerReset;
                        isChasing = true;
                    }
                }
                break;

            case State.isAggro:
                {
                    agent.destination = movePosTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (isChasing)
                    {
                        state = State.isPatrolling;
                    }

                    if (boombugExplode.explosionMode)
                    {
                        state = State.isExploding;
                    }
                }
                break;
            
            case State.isExploding:
                {
                    agent.destination = transform.position;
                }
                break;
        }
        
    }
}
