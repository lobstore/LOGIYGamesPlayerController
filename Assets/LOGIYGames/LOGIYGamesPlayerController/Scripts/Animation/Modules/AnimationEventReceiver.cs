using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class AnimationEventReceiver
        : MonoBehaviour
    {
        private CharacterModule character;

        private void Awake()
        {
            character =
                GetComponent<CharacterModule>();
        }

        // ========================================================
        // ABILITY
        // ========================================================

        public void AbilityStarted()
        {
            SendEvent(
                AnimationEventType
                    .AbilityStarted);
        }

        public void AbilityActionStarted()
        {
            SendEvent(
                AnimationEventType
                    .AbilityActionStart);
        }

        public void AbilityActionEnded()
        {
            SendEvent(
                AnimationEventType
                    .AbilityActionEnd);
        }

        public void AbilityFinished()
        {
            SendEvent(
                AnimationEventType
                    .AbilityFinished);
        }

        // ========================================================
        // HITBOX
        // ========================================================

        public void EnableHitbox()
        {
            SendEvent(
                AnimationEventType
                    .EnableHitbox);
        }

        public void DisableHitbox()
        {
            SendEvent(
                AnimationEventType
                    .DisableHitbox);
        }

        // ========================================================
        // COMBO
        // ========================================================

        public void OpenComboWindow()
        {
            SendEvent(
                AnimationEventType
                    .OpenComboWindow);
        }

        public void CloseComboWindow()
        {
            SendEvent(
                AnimationEventType
                    .CloseComboWindow);
        }
        public void FinishAttack()
        {
            SendEvent(
                AnimationEventType
                    .AttackFinished);
        }
        // ========================================================
        // CANCEL
        // ========================================================

        public void EnableCancel()
        {
            SendEvent(
                AnimationEventType
                    .OpenCancelWindow);
        }

        public void DisableCancel()
        {
            SendEvent(
                AnimationEventType
                    .CloseCancelWindow);
        }

        // ========================================================
        // SEND
        // ========================================================

        private void SendEvent(
            AnimationEventType type)
        {
            character.EventBus.Publish(
                new Shared.Character.Events
                    .CharacterAnimationEvent()
                {
                    AnimationEventType = type
                });
        }
    }

}
