using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShellShockMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;
    [SerializeField] public Transform movePosTransform;
    AnimationShellshockScript shellAnimation;

    public NavMeshAgent agent;
    [SerializeField] private float margin = 5f;
    [SerializeField] public float distanceToTarget;
    [SerializeField] public float distanceToPlayer;
    [SerializeField]private float distanceToStartAttacking = 10f;
    [SerializeField] private float distancetoStartAggro = 14f;
    [SerializeField] private float stopChasingTimer = 0f;
    [SerializeField] private float stopChasingTimerReset = 5f;
    [SerializeField] private float stopShootingTimer = 0f;
    [SerializeField] private float stopShootingTimerReset = 6f;
    private float rotateSpeed = 5f;
    [SerializeField] bool reachedDestination = false;
    [SerializeField] bool isChasing;

    public LayerMask obstacleMask;

    EnemyDetection enemyDetection;
    Shooting shooting;
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
        shellAnimation = GetComponent<AnimationShellshockScript>();
        shooting = GetComponent<Shooting>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, movePosTransform.position);

        switch (state)
        {
            case State.isPatrolling:
                {
                    shellAnimation.walking = true;

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
                        stopChasingTimer = stopChasingTimerReset;
                        isChasing = true;
                        shellAnimation.walking = false;
                        state = State.isAggro;
                    }
                }
                break;

            case State.isAggro:
                {
                    shellAnimation.running = true;

                    agent.destination = movePosTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (distanceToPlayer < distanceToStartAttacking && enemyDetection.detected)
                    {
                        state = State.isAttacking;
                        stopShootingTimer = stopShootingTimerReset;
                        shellAnimation.running = false;
                    }

                     StopChasing();
                }
                break;

            case State.isAttacking:
                {
                    shellAnimation.attacking=true;

                    agent.destination = transform.position;
                    LookAtPlayer();

                    shooting.shootingAtPlayer = true;
                    if (distanceToPlayer > distancetoStartAggro || stopShootingTimer <= 0)
                    {
                        shooting.shootingAtPlayer=false;
                        shellAnimation.attacking = false;
                        state = State.isAggro;
                    }
                    else if (!enemyDetection.detected)
                    {
                        stopShootingTimer -= Time.deltaTime;
                    }

                }
                break;
        }

    }
    private void LookAtPlayer()
    {
        direction = (movePosTransform.position - transform.position);
        direction.y = 0;
        direction = direction.normalized;
        Quaternion rotGoal = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
        transform.rotation = rotation;
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
            Debug.Log("ShellShock stopped chasing you");
        }

        if (!isChasing)
        {
            state = State.isPatrolling;
        }
    }
}
