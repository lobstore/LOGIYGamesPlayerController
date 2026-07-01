using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] UnityEvent OnLFoot = new();
        [SerializeField] UnityEvent OnRFoot = new();
        [SerializeField] UnityEvent OnRHandAttack = new();
        [SerializeField] UnityEvent OnLHandAttack = new();
        [SerializeField] private Animator animator;
        private Character character;

        private void Awake()
        {
            character = GetComponent<Character>();
        }

        public void RFootStep(UnityEngine.AnimationEvent @event)
        {
            if (IsHeaviestAnimClip(@event.animatorClipInfo.clip))
            {
                OnRFoot?.Invoke();
            }
        }
        public void LFootStep(UnityEngine.AnimationEvent @event)
        {
            if (IsHeaviestAnimClip(@event.animatorClipInfo.clip))
            {
                OnLFoot?.Invoke();
            }
        }
        bool IsHeaviestAnimClip(AnimationClip currentClip)
        {
            var currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            float highestWeight = 0f;
            AnimationClip highestWeightClip = null;

            // Find the clip with the highest weight
            foreach (var clipInfo in currentAnimatorClipInfo)
            {
                if (clipInfo.weight > highestWeight)
                {
                    highestWeight = clipInfo.weight;
                    highestWeightClip = clipInfo.clip;
                }
            }

            return highestWeightClip != null && currentClip == highestWeightClip;
        }

        public void RHandAttack()
        {
            OnRHandAttack?.Invoke();
        }
        public void LHandAttack()
        {
            OnLHandAttack?.Invoke();
        }

    }

}
