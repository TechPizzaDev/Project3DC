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
    [SerializeField] public float distanceToPlayer;
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

        distanceToPlayer = Vector3.Distance(transform.position, movePosTransform.position);
        if (!enemyDetection.detected)
        {
            state = State.isPatrolling;
        }

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
                    }
                }
                break;

            case State.isAggro:
                {
                    agent.destination = movePosTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (distanceToPlayer < 10)
                    {
                        state = State.isAttacking;
                    }
                }
                break;

            case State.isAttacking:
                {
                    agent.destination = transform.position;
                    Vector3 targetVector = movePosTransform.position - transform.position;
                    var targetAngle = Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0 ,targetAngle, 0);

                    if (distanceToPlayer > 12)
                    {
                        state = State.isAggro;
                    }
                }
                break;
        }

    }
}
