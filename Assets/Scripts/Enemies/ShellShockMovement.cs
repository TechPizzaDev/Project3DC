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
    [SerializeField] private float distanceToStartAttacking = 10f;
    [SerializeField] private float distancetoStartAggro = 14f;
    [SerializeField] private float stopChasingTimer = 0f;
    [SerializeField] private float stopChasingTimerReset = 5f;
    [SerializeField] private float stopShootingTimer = 0f;
    [SerializeField] private float stopShootingTimerReset = 6f;
    private float rotateSpeed = 5f;
    [SerializeField] bool reachedDestination = false;
    [SerializeField] bool isChasing;
    [SerializeField] bool reloadAnimationDone;

    public LayerMask obstacleMask;

    EnemyDetection enemyDetection;
    EnemyHealth health;
    float aggressiveSpeed = 3.0f;
    float regularSpeed = 1.0f;

    enum State
    {
        isPatrolling,
        isAggro,
        isAttacking,
        killed
    }
    State state;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        agent = GetComponent<NavMeshAgent>();
        shellAnimation = GetComponent<AnimationShellshockScript>();
        health = GetComponent<EnemyHealth>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, movePosTransform.position);

        if (health.killed)
        {
            state = State.killed;
        }

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
                        shellAnimation.running = true;
                        shellAnimation.walking = false;
                        state = State.isAggro;
                    }
                }
                break;

            case State.isAggro:
                {

                    agent.destination = movePosTransform.position;

                    agent.speed = aggressiveSpeed;

                    if (distanceToPlayer < distanceToStartAttacking && enemyDetection.detected)
                    {
                        shellAnimation.running = false;
                        shellAnimation.attacking = true;
                        state = State.isAttacking;
                        stopShootingTimer = stopShootingTimerReset;
                    }

                     StopChasing();
                }
                break;

            case State.isAttacking:
                {
                    agent.destination = transform.position;
                    LookAtPlayer();

                    if(distanceToPlayer > distancetoStartAggro || stopShootingTimer <= 0)
                    {
                        if (reloadAnimationDone)
                        {
                            shellAnimation.attacking = false;
                            shellAnimation.running = true;
                            state = State.isAggro;
                        }

                    }
                    reloadAnimationDone = false;

                    if (!enemyDetection.detected)
                    {
                        stopShootingTimer -= Time.deltaTime;
                    }
                    else
                    {
                        stopShootingTimer = stopShootingTimerReset;
                    }

                }
                break;

            case State.killed:
                {
                    agent.destination = transform.position;

                    shellAnimation.die = true;
                    shellAnimation.attacking = false;
                    shellAnimation.running = false;
                    shellAnimation.walking = false;
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
            shellAnimation.running = false;
            state = State.isPatrolling;
        }
    }
    public void ReloadAnimationDone()
    {
        reloadAnimationDone = true;
    }
}
