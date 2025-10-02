using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    [System.Serializable]
    public class AnimatorMoveEvent : UnityEvent<Vector3, Quaternion> { }
    public class AnimatorEvents : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnShoot = new UnityEvent();
        public UnityEvent OnFootR = new UnityEvent();
        public UnityEvent OnFootL = new UnityEvent();
        public UnityEvent OnLand = new UnityEvent();
        public UnityEvent OnWeaponSwitch = new UnityEvent();

        public AnimatorMoveEvent OnMove = new AnimatorMoveEvent();


        public void Hit() => OnHit.Invoke();
        public void Shoot() => OnShoot.Invoke();
        public void FootR(AnimationEvent evt)
        {
            if (evt.animatorClipInfo.weight > 0.5)
            {
                OnFootR.Invoke();
            }
        }
        public void FootL(AnimationEvent evt)
        {
            if (evt.animatorClipInfo.weight > 0.5)
            {
                OnFootL.Invoke();
            }
        }
        public void Land() => OnLand.Invoke();

        public void WeaponSwitch() => OnWeaponSwitch.Invoke();

    }
}