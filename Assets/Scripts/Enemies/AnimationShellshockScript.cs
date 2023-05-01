using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationShellshockScript : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    public bool walking, running, attacking, die;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        Killed();

        if (walking)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        if (running)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if (attacking)
        {
            animator.SetBool("Attacking", true);
        }
        else
        {
            animator.SetBool("Attacking", false);
        }

    }
    private void StandingStill()
    {
        animator.SetBool("Standing", true);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
        animator.SetBool("Attacking", false);
    }
    private void Killed()
    {

        if (die)
        {
            animator.SetBool("Killed", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);
            animator.SetBool("Attacking", false);
        }
    }
}
