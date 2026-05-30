using LOGIYGames.Shared.Character.Events;
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
            SendAbilityAnimationEvent(
                AbilityEventType
                    .Started);
        }



        public void AbilityActionStarted()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .ActionStart);
        }

        public void AbilityActionEnded()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .ActionEnd);
        }

        public void AbilityFinished()
        {
            SendAbilityAnimationEvent(
                AbilityEventType
                    .Finished);
        }

        // ========================================================
        // HITBOX
        // ========================================================

        public void EnableHitbox()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .EnableHitbox);
        }

        public void DisableHitbox()
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

        public void EnableCancel()
        {
            SendComboAnimationEvent(
                ComboEventType
                    .OpenCancelWindow);
        }

        public void DisableCancel()
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
            character.EventBus.Publish(new AbilityTimedEvent()
            {
                AbilityEventType = type
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
