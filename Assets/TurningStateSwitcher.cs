using UnityEngine;

namespace LOGIYGames
{
    public class TurningStateSwitcher : StateMachineBehaviour
    {
        LocomotionActionContext action_context;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (action_context == null)
            {
                action_context = animator.GetComponent<LocomotionActionContext>();
            }
            action_context.IsTurning = true;
            action_context.CanMove = false;
            if (action_context.MotionType != MotionType.AnimatorController)
            {
                animator.applyRootMotion = true;
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (action_context == null)
            {
                action_context = animator.GetComponent<LocomotionActionContext>();
            }
            action_context.IsTurning = false;
            action_context.CanMove = true;
            if (action_context.MotionType == MotionType.AnimatorController)
            {
                animator.applyRootMotion = true;
            }
            else
            {
                animator.applyRootMotion = false;
            }

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
}
