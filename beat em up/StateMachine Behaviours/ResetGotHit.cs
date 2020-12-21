using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGotHit : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("GetUp", false);
        animator.SetBool("Revive", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //  animator.SetBool("GotHit", false);
        animator.transform.root.GetComponent<Entity>().allowed = true;
        animator.transform.root.GetComponent<Entity>().floored = false;
        animator.SetInteger("hitStack", 0);
        animator.transform.root.GetComponent<Entity>().hitStack = 0;
        animator.SetBool("Floored", false);
        

//        animator.transform.GetChild(0).gameObject.SetActive(true);
        animator.transform.root.GetComponent<Entity>().setBlock();

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
