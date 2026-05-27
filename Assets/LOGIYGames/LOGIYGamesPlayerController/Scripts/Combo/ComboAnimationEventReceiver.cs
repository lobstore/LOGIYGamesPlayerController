using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboAnimationEventReceiver
    : MonoBehaviour
    {
        private CharacterModule character;

        private void Awake()
        {
            character =
                GetComponent<CharacterModule>();
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

        // ========================================================
        // ATTACK END
        // ========================================================

        public void FinishAttack()
        {

            SendEvent(
    AnimationEventType
        .EndAnimation);

        }
        public void EnableCancel()
        {
            SendEvent(
                AnimationEventType
                    .EnableCancelWindow);
        }

        public void DisableCancel()
        {
            SendEvent(
                AnimationEventType
                    .DisableCancelWindow);
        }
        // ========================================================
        // SEND
        // ========================================================

        private void SendEvent(
            AnimationEventType type)
        {
            character.EventBus.Publish(new Shared.Character.Events.AnimationEvent()
            {
                AnimationEventType = type
            });
        }
    }
}
