using Alchemy.Inspector;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class JumpController : MonoBehaviour
    {
        [field: SerializeField][field: ReadOnly] public int JumpCount {  get; private set; }
        [field:SerializeField] public int MaxJumpCount {  get; private set; }
        [SerializeField] Actor Character;
        private void Awake()
        {
            EventsSubscription();
        }
        public void ResetJumps()
        {
            JumpCount = 0;
        }
        public bool CanExecute(JumpStateData jumpStateData)
        {
            return JumpCount < MaxJumpCount && Character.StaminaController.TryUse(jumpStateData.StaminaUsage);
        }
        private void EventsSubscription()
        {
            Character.EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.HangJump:
                        Character.Jump(Character.Sensors.LegsFrontHit.normal * evt.planarForce + evt.verticalForce * Character.transform.up);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    case JumpType.Roll:
                        Character.Jump(Character.transform.forward * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Dash:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Slip:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce);
                        break;
                    default:
                        break;
                }
                JumpCount++;
            });
        }
    }
}
