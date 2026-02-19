using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class SetBool : StateMachineBehaviour
    {
        [SerializeField] InterruptType interruptType;
        [SerializeField] string parameterName;
        [SerializeField] bool enabled;
        int parameterHash;

        private void Awake()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (interruptType == InterruptType.OnStateExit)
            {
                if (!enabled)
                {
                    animator.SetBool(parameterHash, false);
                }
                else
                {
                    animator.SetBool(parameterHash, true);
                }
            }

        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (interruptType == InterruptType.OnStateUpdate)
            {
                if (!enabled)
                {
                    animator.SetBool(parameterHash, false);
                }
                else
                {
                    animator.SetBool(parameterHash, true);
                }
            }
        }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (interruptType == InterruptType.OnStateEnter)
            {
                if (!enabled)
                {
                    animator.SetBool(parameterHash, false);
                }
                else
                {
                    animator.SetBool(parameterHash, true);
                }
            }
        }
    }
}