using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BoombugMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;

    public NavMeshAgent agent;
    [SerializeField] private float margin = 5f;
    [SerializeField] public float distanceToTarget;
    [SerializeField] private float stopChasingTimer = 0f;
    [SerializeField] private float stopChasingTimerReset = 5f;
    [SerializeField] bool reachedDestination = false;
    bool isChasing;

    EnemyDetection enemyDetection;
    BoombugExplode boombugExplode;
    UnitHealth health;
    float aggressiveSpeed = 3.0f;
    float regularSpeed = 1.0f;

    enum State
    {
        isPatrolling,
        isAggro,
        isExploding,
        killed
    }
    State state;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        boombugExplode = GetComponent<BoombugExplode>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<UnitHealth>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {
        //If enemys health bar reached 0, its dies
        if (health.CurrentHealth <= 0)
        {
            state = State.killed;
        }


        switch (state)
        {
            case State.isPatrolling:
                {
                    //Checks the distence to enemys destination and sets the speed
                    distanceToTarget = Vector3.Distance(transform.position, walkingToPos);
                    agent.destination = walkingToPos;
                    agent.speed = regularSpeed;

                    //If the enemy have reached the destination, set new location
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

                    //If enemy detects the player, set the enemy state to aggresiv and start chasing
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
                    //Sets enemys destiantion to the players position and sets the speed
                    agent.destination = enemyDetection.targetTransform.position;
                    agent.speed = aggressiveSpeed;

                    //If the enemy stops chasing it goes back to its patroling state again
                    if (!isChasing)
                    {
                        state = State.isPatrolling;
                    }

                    //If enemy is close enough, set enemy state to exploding mode
                    if (boombugExplode.explosionMode)
                    {
                        state = State.isExploding;
                    }

                    //Checks if the enemy should stop chasing the player
                    StopChasing();
                }
                break;

            case State.isExploding:
                {
                    //Makes the enemy stand still
                    agent.destination = transform.position;
                }
                break;

            case State.killed:
                {
                    Destroy(gameObject);
                }
                break;
        }

    }
    /// <summary>
    /// Checks if the enemy should stop chasing the player and go back to its patroling states
    /// </summary>
    private void StopChasing()
    {
        //If enemy detects player, restart the timer for stop chasing the player
        if (enemyDetection.detected)
        {
            stopChasingTimer = stopChasingTimerReset;
        }

        //Count down to stop chasing the player, if ir reaches 0 it stops
        if (stopChasingTimer > 0f)
        {
            stopChasingTimer -= Time.deltaTime;
        }
        else
        {
            isChasing = false;
            Debug.Log("Boombug stopped chasing you");
        }

        //Sets the enemy state to back to patrolling again
        if (!isChasing)
        {
            state = State.isPatrolling;
        }
    }
}
