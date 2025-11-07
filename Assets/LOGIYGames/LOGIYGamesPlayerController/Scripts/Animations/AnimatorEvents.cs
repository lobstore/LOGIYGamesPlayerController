using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    [System.Serializable]
    public class AnimatorMoveEvent : UnityEvent<Vector3, Quaternion> { }
    public class AnimatorEvents : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public UnityEvent OnFootR = new UnityEvent();
        public UnityEvent OnFootL = new UnityEvent();
        public UnityEvent OnLand = new UnityEvent();
        UnityControllerWrapper RootMotionApplicator;
        Animator Animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            RootMotionApplicator = GetComponent<UnityControllerWrapper>();
        }

        public void FootR(AnimationEvent evt)
        {
            if (evt.animatorClipInfo.weight > 0.35)
            {
                OnFootR.Invoke();
            }
        }
        public void FootL(AnimationEvent evt)
        {
            if (evt.animatorClipInfo.weight > 0.35)
            {
                OnFootL.Invoke();
            }
        }
        private void OnAnimatorMove()
        {
            
        }
    }
}