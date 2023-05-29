using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles enemy detection logic based on the enemy's field of view.
/// It calculates the angle and distance between the enemy and the target (typically the player),
///  checks for obstacles in the line of sight, and determines if the enemy has detected the target.
/// </summary>
/// 
public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private float viewRadius;
    [SerializeField] private float viewAngle;
    public bool detected;
    public bool awoken;

    public LayerMask obstacleMask;

    public Transform targetTransform;

    void Start()
    {
        // Sets the target transform to the player's transform if it is not assigned
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Responsible for angle of target & enemy
        Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

        // Checks if enemies forward position & target is less than the viewing angle / 2.
        // Divide by 2 to create 45 degrees towards both left and right.
        if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 || detected)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
            if (distanceToTarget <= viewRadius)
            {
                //If there is no obstacle in the way = enemy has detected the player. 
                if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, obstacleMask))
                {
                    if (!detected)
                    {
                        detected = true;
                        Debug.Log("You have been spotted.");
                    }
                    return;
                }
            }
        }
        // Resets the detected state to false if the target is not detected
        detected = false;
    }
}