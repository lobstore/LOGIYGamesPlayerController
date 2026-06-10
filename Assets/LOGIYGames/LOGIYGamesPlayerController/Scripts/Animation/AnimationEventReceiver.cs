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
        private CharacterModule character;

        private void Awake()
        {
            character = GetComponent<CharacterModule>();
        }

        public void RFootStep(AnimationEvent @event)
        {
            if (IsHeaviestAnimClip(@event.animatorClipInfo.clip))
            {
                OnRFoot?.Invoke();
            }
        }
        public void LFootStep(AnimationEvent @event)
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
        // ========================================================
        // ABILITY
        // ========================================================

        public void AbilityStarted()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .Started);
        }
        public void AbilityActionStarted()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .AnimationStart);
        }
        public void AbilityActionEnded()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .AnimationEnd);
        }
        public void AbilityFinished()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .Finished);
        }

        // ========================================================
        // ATTACK
        // ========================================================

        public void RHandAttack()
        {
            OnRHandAttack?.Invoke();
        }
        public void LHandAttack()
        {
            OnLHandAttack?.Invoke();
        }
        public void OpenHitWindow()
        {

            SendComboAnimationEvent(
                ComboEventType
                    .EnableHitbox);
        }

        public void CloseHitWindow()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .DisableHitbox);
        }

        // ========================================================
        // COMBO
        // ========================================================

        public void OpenComboWindow()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .OpenComboWindow);
        }

        public void CloseComboWindow()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .CloseComboWindow);
        }
        public void FinishAttack()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .AttackFinished);
        }
        // ========================================================
        // CANCEL
        // ========================================================

        public void EnableCancelationWindow()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .OpenCancelWindow);
        }

        public void DisableCancelationWindow()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .CloseCancelWindow);
        }

        // ========================================================
        // SEND
        // ========================================================

        private void SendAbilityAnimationEvent(AbilityEventType type)
        {
            character.EventBus.Publish(new AnimationTimedEvent()
            {
                // AbilityEventType = type

            });
        }
        private void SendComboAnimationEvent(ComboEventType type)
        {
            character.EventBus.Publish(new ComboAnimationEvent()
            {
                ComboEventType = type
            });
        }
    }

}
