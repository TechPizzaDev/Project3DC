using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    EnemyDetection detection;

    void Start()
    {
        animator = GetComponent<Animator> ();  
        detection = GetComponent<EnemyDetection> ();
    }

    void Update()
    {
        if (detection.detected)
        {
            animator.SetBool("Roll_Animation", true);
            animator.SetBool("Walking_Animation", false);
        }
        if(!detection.detected) 
        {
            animator.SetBool("Walking_Animation", true);
            animator.SetBool("Roll_Animation", false);


        }
    }
}
