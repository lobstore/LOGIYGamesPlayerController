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
        MovementWrapperBase Motor;
        Actor Character;
        SensorsModule Sens;
        private void Awake()
        {
            Sens = GetComponent<SensorsModule>();
            Motor = GetComponent<MovementWrapperBase>();
            Character = GetComponent<Actor>();
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

                Motor.ResetGravity();
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.HangJump:
                        Motor.AddImpulse(Character.Sensors.LegsFrontHit.normal * evt.planarForce + evt.verticalForce * Character.transform.up);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    case JumpType.Roll:
                        Motor.AddImpulse(Character.transform.forward * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Dash:
                        Motor.AddImpulse(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Slip:
                        Motor.AddImpulse(Character.transform.forward * evt.planarForce);
                        break;
                    default:
                        break;
                }
                JumpCount++;
            });
            Sens.GroundedEvent.AddListener(_ => ResetJumps());
        }
    }
}
