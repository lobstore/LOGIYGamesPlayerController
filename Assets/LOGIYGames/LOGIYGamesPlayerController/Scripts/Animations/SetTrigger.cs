using UnityEngine;
namespace LOGIYGames
{
    public class SetTrigger : StateMachineBehaviour
    {
        [SerializeField] InterruptType interruptType;
        [SerializeField] string parameterName;
        [SerializeField] bool setOn;
        int parameterHash;

        private void Awake()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (interruptType == InterruptType.OnStateExit)
                if (setOn)
                {
                    animator.SetTrigger(parameterHash);
                }
                else
                {
                    animator.ResetTrigger(parameterHash);
                }
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (interruptType == InterruptType.OnStateUpdate)
            {
                if (setOn)
                {
                    animator.SetTrigger(parameterHash);
                }
                else
                {
                    animator.ResetTrigger(parameterHash);
                }
            }
        }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (interruptType == InterruptType.OnStateEnter)
                if (setOn)
                {
                    animator.SetTrigger(parameterHash);
                }
                else
                {
                    animator.ResetTrigger(parameterHash);
                }
        }
    }
}