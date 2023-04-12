using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator anim;
    EnemyMovement enemyMovement;

    // Use this for initialization
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        enemyMovement = gameObject.GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
    }

    void CheckKey()
    {
        // Walk
        if (enemyMovement.animationWalking)
        {
            anim.SetBool("Walk_Anim", true);
        }
        else if (!enemyMovement.animationWalking)
        {
            anim.SetBool("Walk_Anim", false);

        }

        // Roll
        if (enemyMovement.animationRolling)
        {
            
            anim.SetBool("Roll_Anim", true);
            
        }

        //// Close
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    if (!anim.GetBool("Open_Anim"))
        //    {
        //        anim.SetBool("Open_Anim", true);
        //    }
        //    else
        //    {
        //        anim.SetBool("Open_Anim", false);
        //    }
        //}
    }

}
