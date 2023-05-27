using UnityEngine;
using UnityEngine.AI;

public class ShellShockMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 walkingToPos;
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

    EnemyDetection enemyDetection;
    UnitHealth health;
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
        health = GetComponent<UnitHealth>();

        walkingToPos = endPos;
        startingPos = transform.position;
        state = new State();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = enemyDetection.targetTransform.position;
        distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

        if (health.CurrentHealth <= 0)
        {
            state = State.killed;
        }

        switch (state)
        {
            case State.isPatrolling:
                {
                    shellAnimation.walking = true;

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
                    //Sets enemys destiantion to the players position and sets the speed
                    agent.destination = targetPosition;
                    agent.speed = aggressiveSpeed;

                    //If the enemy is close enough, set enemy state to attacking and start shooting the player
                    if (distanceToPlayer < distanceToStartAttacking && enemyDetection.detected)
                    {
                        shellAnimation.running = false;
                        shellAnimation.attacking = true;
                        state = State.isAttacking;
                        stopShootingTimer = stopShootingTimerReset;
                    }

                    //Checks if the enemy should stop chasing the player
                    StopChasing();
                }
                break;

            case State.isAttacking:
                {
                    //Makes the enemy stand still and just rotate towards the player
                    agent.destination = transform.position;
                    LookAtPlayer();

                    //If the player is to farm from the player or if the player is not in line of sigt for to long, it start to run path towards the player 
                    if (distanceToPlayer > distancetoStartAggro || stopShootingTimer <= 0)
                    {
                        //Enemy needs to do the reloading animation done before it can start chasing after again
                        if (reloadAnimationDone)
                        {
                            shellAnimation.attacking = false;
                            shellAnimation.running = true;
                            state = State.isAggro;
                        }

                    }
                    reloadAnimationDone = false;

                    //If the enemy can see the player the stop shooting timer starts
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
                    //Makes the enemy stand still
                    agent.destination = transform.position;

                    shellAnimation.die = true;
                    shellAnimation.attacking = false;
                    shellAnimation.running = false;
                    shellAnimation.walking = false;
                }
                break;

        }

    }
    /// <summary>
    /// Makes the enemy rotate towards the player
    /// </summary>
    private void LookAtPlayer()
    {
        direction = enemyDetection.targetTransform.position - transform.position;
        direction.y = 0;
        direction = direction.normalized;

        Quaternion rotGoal = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
        transform.rotation = rotation;
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
            Debug.Log("ShellShock stopped chasing you");
        }

        //Sets the enemy state to back to patrolling again
        if (!isChasing)
        {
            shellAnimation.running = false;
            state = State.isPatrolling;
        }
    }

    /// <summary>
    /// Needed to en Animation Event for the reload to check if the enemy is reloading
    /// </summary>
    public void ReloadAnimationDone()
    {
        reloadAnimationDone = true;
    }
}
