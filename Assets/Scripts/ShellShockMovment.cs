using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShellShockMovment : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;
    [SerializeField] public Transform movePosTransform;

    public NavMeshAgent agent;
    [SerializeField] private float margin = 5f;
    [SerializeField] public float distanceToTarget;
    [SerializeField] bool reachedDestination = false;

    EnemyDetection enemyDetection;
    float aggressiveSpeed = 3.0f;
    float regularSpeed = 1.0f;

    enum State
    {
        isPatrolling,
        isAggro,
        isAttacking
    }
    State state;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        agent = GetComponent<NavMeshAgent>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {

        distanceToTarget = Vector3.Distance(transform.position, walkingToPos);

        switch (state)
        {
            case State.isPatrolling:
                {
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
                    }
                }
                break;

            case State.isAggro:
                {
                    agent.destination = movePosTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (!enemyDetection.detected)
                    {
                        state = State.isPatrolling;
                    }

                    if (distanceToTarget < 10)
                    {
                        state = State.isAttacking;
                    }
                }
                break;

            case State.isAttacking:
                {
                    agent.destination = transform.position;

                    if (distanceToTarget > 15)
                    {
                        state = State.isAggro;
                    }
                }
                break;
        }

    }
}
