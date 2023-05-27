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
        if (health.CurrentHealth <= 0)
        {
            state = State.killed;
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
                        stopChasingTimer = stopChasingTimerReset;
                        isChasing = true;
                    }
                }
                break;

            case State.isAggro:
                {
                    agent.destination = enemyDetection.targetTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (!isChasing)
                    {
                        state = State.isPatrolling;
                    }

                    if (boombugExplode.explosionMode)
                    {
                        state = State.isExploding;
                    }
                    StopChasing();
                }
                break;

            case State.isExploding:
                {
                    agent.destination = transform.position;
                }
                break;

            case State.killed:
                {
                    health.DestroyObj();
                }
                break;
        }

    }
    private void StopChasing()
    {
        if (enemyDetection.detected)
        {
            stopChasingTimer = stopChasingTimerReset;
        }

        if (stopChasingTimer > 0f)
        {
            stopChasingTimer -= Time.deltaTime;
        }
        else
        {
            isChasing = false;
            Debug.Log("Boombug stopped chasing you");
        }

        if (!isChasing)
        {
            state = State.isPatrolling;
        }
    }
}
